using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PetSociety.API.DTOs.Shop;
using PetSociety.API.Models;
using System.Buffers.Text;


namespace PetSociety.API.Services.Shop
{
    public class OrderService : IOrderService
    {
        private readonly PetSocietyContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(PetSocietyContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor; 
        }
        public async Task<Order> CreateOrderAsync(CreateOrderDto request)
        {
            // 產生不重複的訂單編號 (MerchantTradeNo)
            // 規則： "SHOP" + 年月日時分秒 + 3碼亂數 (例如：SHOP20251230120000123)
            // 這樣給藍新才不會重複
            string tradeNo = $"SHOP{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";

            var newOrder = new Order
            {
                // 左邊是「資料庫欄位」，右邊是「前端傳來的 DTO」
                MemberId = request.MemberId,
                UserCouponId = request.UserCouponId, // 可為 Null

                // 這裡要注意！雖然前端有傳 TotalAmount，但嚴謹的做法應該是：
                // 去撈 Products 資料表算單價 * 數量 - 折扣 = FinalAmount
                // 我們先信任前端傳來的金額
                FinalAmount = request.TotalAmount,

                OrderDate = DateTime.Now, // 下單時間

                // 狀態初始設定
                OrderStatus = "Pending",   // 處理中
                PaymentStatus = "Unpaid",  // 未付款
                PaymentMethod = request.PaymentMethod,

                // 金流編號
                MerchantTradeNo = tradeNo,
                TradeNo = null,   // 藍新還沒回傳，所以是 Null
                TradeDate = null, // 還沒付錢，所以是 Null

                // 收件人資訊 (從 DTO 塞進來)
                ReceiverName = request.Receiver.Name,
                ReceiverPhone = request.Receiver.Phone,
                ReceiverEmail = request.Receiver.Email,
                ShippingAddress = request.Receiver.Address
            };

            // 準備訂單明細 (OrderDetails)
            // 我們要把 DTO 裡的 List<OrderItemDto> 轉成 List<OrderDetail>
            foreach (var item in request.OrderItems)
            {
                var detail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice // 這裡一樣建議之後改為後端查價
                };
                newOrder.OrderDetails.Add(detail);
            }

            // 存入資料庫
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // 找出這個會員購物車裡的所有東西
            var cartItems = _context.ShoppingCarts.Where(c => c.MemberId == request.MemberId);

            // 全部刪除
            _context.ShoppingCarts.RemoveRange(cartItems);

            // 再次存檔
            await _context.SaveChangesAsync();

            // 回傳建立好的訂單 (讓 Controller 之後可以拿去呼叫藍新)
            return newOrder;
        }
        // 根據 MemberId 查詢歷史訂單
        public async Task<List<OrderResponseDto>> GetOrdersByMemberIdAsync(int memberId)
        {
            string baseUrl = "https://localhost:7032";

            // 第一階段：去資料庫搬貨 (只做 SQL 能做的事)
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.ProductImages) // 把圖片庫關聯進來
                .Where(o => o.MemberId == memberId && o.OrderStatus != "Deleted") // 修改：這裡多加一個判斷，不撈出狀態是 "Deleted" 的訂單
                .OrderByDescending(o => o.OrderDate);

            // ★ 關鍵時刻：加上 await ToListAsync()，把資料真正搬回你的伺服器記憶體！
            var orders = await query.ToListAsync();

            // 第二階段：加工處理 (在記憶體執行)
            // 因為資料已經搬回來了，這裡可以用 C# 的 Convert.ToBase64String，絕對不會報錯！
            var result = orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.FinalAmount,
                PaymentDate = o.TradeDate,
                Status = o.OrderStatus,
                ReceiverName = o.ReceiverName,
                ReceiverAddress = o.ShippingAddress,
                ReceiverPhone = o.ReceiverPhone,

                // 轉換明細
                Details = o.OrderDetails.Select(d => new OrderDetailResponseDto
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product.ProductName,
                    Quantity = d.Quantity,
                    Price = d.UnitPrice,

                    Thumbnail = d.Product.ProductImages.OrderBy(img => img.ImageId).FirstOrDefault()?.ImagePath != null
                        ? $"{baseUrl}/{d.Product.ProductImages.OrderBy(img => img.ImageId).First().ImagePath}"
                        : null

                }).ToList()

            }).ToList();

            return result;
        }
        public async Task HandlePaymentSuccessAsync(string merchantOrderNo, string tradeNo, string paymentDate)
        {
            // 1. 撈出這筆訂單
            // 重點：一定要加 .Include(o => o.OrderDetails)，不然等一下抓不到買了什麼商品，就不能扣庫存！
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.MerchantTradeNo == merchantOrderNo);

            // 如果找不到訂單，就直接結束
            if (order == null) return;

            // 2. 檢查訂單狀態 (避免藍新重複通知導致重複扣庫存)
            // 只有當狀態是 "Unpaid" 的時候才處理
            if (order.PaymentStatus == "Unpaid")
            {
                // (A) 更新訂單狀態
                order.PaymentStatus = "Paid";       // 改成已付款
                order.OrderStatus = "Processing";   // 訂單狀態改為處理中 (或你要叫 Paid 也可以)
                order.TradeNo = tradeNo;            // 填入藍新交易序號 (查帳用)

                // (B) 處理交易時間 (字串轉日期)
                if (DateTime.TryParse(paymentDate, out DateTime payDate))
                {
                    order.TradeDate = payDate;
                }

                // (C) 核心邏輯：扣庫存！
                foreach (var item in order.OrderDetails)
                {
                    // 找出對應的商品
                    var product = await _context.Products.FindAsync(item.ProductId);

                    if (product != null)
                    {
                        // 庫存減去購買數量
                        product.Stock -= item.Quantity;

                        // (選做) 防呆：如果庫存變負的，強制歸零 (雖然前端應該擋過，但後端再擋一次比較保險)
                        if (product.Stock < 0)
                        {
                            product.Stock = 0;
                        }
                    }
                }

                // (D) 最後一次存檔 (同時更新訂單和商品庫存)
                await _context.SaveChangesAsync();
            }

        }
        // 實作介面方法
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            // 去資料庫撈訂單，記得要用 FirstOrDefaultAsync
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            return order;
        }
        public async Task CancelOrderAsync(int orderId, int memberId)
        {
            // 1. 開啟交易模式 (Transaction)
            // 這是為了保護資料，萬一加庫存失敗，狀態修改也要復原
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 2. 撈出訂單 (包含裡面的商品明細 OrderItems)
                var order = await _context.Orders
                    .Include(o => o.OrderDetails) // ★ 重要：一定要 Include 明細，不然不知道要加回什麼商品
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.MemberId == memberId);

                if (order == null) throw new Exception("找不到訂單");

                // 3. 檢查狀態 (只有待付款可以取消)
                if (order.OrderStatus != "Pending")
                {
                    throw new Exception("只有待付款的訂單可以取消");
                }

                // 4. 修改訂單狀態
                order.OrderStatus = "Cancelled";

                // 5. 核心邏輯：恢復庫存
                foreach (var item in order.OrderDetails)
                {
                    // 找出對應的商品
                    var product = await _context.Products.FindAsync(item.ProductId);

                    if (product != null)
                    {
                        // 把數量加回去！
                        product.Stock += item.Quantity;
                    }
                }

                // 6. 存檔並提交交易
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // 如果中間出錯，全部回復原狀
                await transaction.RollbackAsync();
                throw; // 把錯誤往上丟給 Controller 處理
            }
        }
        public async Task DeleteOrderAsync(int orderId, int memberId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.MemberId == memberId);

            if (order == null) throw new Exception("找不到訂單");

            // 只有「已取消 (Cancelled)」或「已完成 (Completed)」的訂單可以刪除
            // (Pending 待付款的不能刪，要先取消才能刪)
            if (order.OrderStatus != "Cancelled" && order.OrderStatus != "Completed")
            {
                throw new Exception("只有已完成或已取消的訂單可以刪除紀錄喔！");
            }

            // 軟刪除核心：只改狀態，不刪資料
            order.OrderStatus = "Deleted"; // 我們約定好 "Deleted" 代表使用者刪除了

            await _context.SaveChangesAsync();
        }
    }
}
