using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetSociety.API.DTOs.Shop;
using PetSociety.API.Extensions;
using PetSociety.API.Models; 
using PetSociety.API.Services.Shop;
using System.Security.Claims; 

namespace PetSociety.API.Controllers.Shop
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration; // 1. 注入設定檔讀取器

        public OrdersController(IOrderService orderService, IConfiguration configuration)
        {
            _orderService = orderService;
            _configuration = configuration;
        }

        // POST: api/orders
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request)
        {
            try
            {            
                request.MemberId = User.GetMemberId();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("無法識別會員身分，請重新登入");
            }
            // 1. 驗證資料
            if (request.OrderItems == null || request.OrderItems.Count == 0)
            {
                return BadRequest("購物車是空的，不能結帳喔！");
            }

            // 2. 呼叫 Service 建立訂單 (資料庫產生訂單，狀態: Pending)
            var order = await _orderService.CreateOrderAsync(request);

            // 3. 開始處理「藍新金流」加密
            // (A) 從 appsettings.json 撈出商店設定
            var merchantID = _configuration["NewebPay:MerchantID"];
            var hashKey = _configuration["NewebPay:HashKey"];
            var hashIV = _configuration["NewebPay:HashIV"];
            var returnUrl = _configuration["NewebPay:ReturnUrl"]; // 付款完成回傳網址 (前端頁面)
            var notifyUrl = _configuration["NewebPay:NotifyUrl"]; // 背景通知網址 (API)

            // (B) 準備傳送給藍新的參數 (字典檔)
            // 這些欄位名稱都是藍新規定的，不能打錯字！
            var tradeParams = new Dictionary<string, string>
            {
                { "MerchantID", merchantID },
                { "RespondType", "JSON" }, // 告訴藍新我們想要 JSON 回應 (雖然 Form Submit 後通常是網頁跳轉)
                { "TimeStamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() },
                { "Version", "2.0" },
                { "MerchantOrderNo", order.MerchantTradeNo }, // 這裡要用 Service 產生的唯一訂單編號
                { "Amt", ((int)order.FinalAmount).ToString() },     // 訂單金額
                { "ItemDesc", "PetSociety 寵物商品" },         // 商品描述 (或是用 request 裡的資訊組裝)
                { "ReturnURL", returnUrl },
                { "NotifyURL", notifyUrl },
                { "Email", request.Receiver?.Email ?? "" },             // 付款人 Email (選填)
                { "LoginType", "0" }, // 0: 不需登入藍新會員
            };

            // (C) 使用我們的 NewebPayCrypto 工具進行加密
            // 步驟一：將參數轉成 QueryString 格式
            string paramString = NewebPayCrypto.ToQueryString(tradeParams);

            // 步驟二：AES 加密 (產生 TradeInfo)
            string tradeInfo = NewebPayCrypto.EncryptAESHex(paramString, hashKey, hashIV);

            // 步驟三：SHA256 壓碼 (產生 TradeSha)
            string rawSha = $"HashKey={hashKey}&{tradeInfo}&HashIV={hashIV}";
            string tradeSha = NewebPayCrypto.EncryptSHA256(rawSha);


            // 目前我們先簡單回傳「訂單建立成功」和「訂單編號」         
            return Ok(new
            {
                Message = "訂單建立成功，正在導向付款頁面...",
                OrderId = order.OrderId, // 我們系統的 ID
                PaymentData = new
                {
                    MerchantID = merchantID,
                    TradeInfo = tradeInfo,
                    TradeSha = tradeSha,
                    Version = "2.0",
                    NewebPayUrl = _configuration["NewebPay:ServiceUrl"]
                }
            });

        }
        // GET /api/orders/my-orders
        [HttpGet("my-orders")] 
        [Authorize] 
        public async Task<ActionResult<List<OrderResponseDto>>> GetMyOrders()
        {
            try
            {             
                int memberId = User.GetMemberId();

                // 呼叫 Service 撈資料
                var orders = await _orderService.GetOrdersByMemberIdAsync(memberId);

                return Ok(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                // 如果丟出 "無效的會員ID"，我們就回傳 401
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                // 其他錯誤回傳 500
                return StatusCode(500, "內部伺服器錯誤");
            }
        }
        // GET: api/orders/payment-info/5
        // 用途：當使用者訂單成立但未付款（或付款失敗）時，重新取得藍新金流的加密資料
        [HttpGet("payment-info/{orderId}")]
        [Authorize] // 記得要登入才能查
        public async Task<ActionResult<object>> GetPaymentInfo(int orderId)
        {
            try
            {
                // 1. 驗證是不是本人的訂單 (安全檢查)
                int memberId = User.GetMemberId();

                // 去資料庫找這筆訂單
                // 條件：訂單ID要對，而且必須是這個會員的 (防止偷看別人的訂單)
                // 我們只撈必要的欄位就好，不用 Include 全部
                var order = await _orderService.GetOrderByIdAsync(orderId);      

                if (order == null) return NotFound("找不到訂單");
                if (order.MemberId != memberId) return Forbid("這不是您的訂單"); // 403 禁止

                // 2. 只有「Pending (待付款)」的訂單才能付款
                // 如果已經付過了 (Paid) 或取消了 (Cancelled)，就不能再付
                if (order.OrderStatus != "Pending")
                {
                    return BadRequest("此訂單狀態不需要付款 (可能已付款或已取消)");
                }

                // 3. 準備藍新金流參數 (跟 CreateOrder 裡面的邏輯一模一樣！)
                // (A) 讀取設定
                var merchantID = _configuration["NewebPay:MerchantID"];
                var hashKey = _configuration["NewebPay:HashKey"];
                var hashIV = _configuration["NewebPay:HashIV"];
                var returnUrl = _configuration["NewebPay:ReturnUrl"];
                var notifyUrl = _configuration["NewebPay:NotifyUrl"];

                // (B) 組裝參數
                // 注意：這裡是補救付款，所以 MerchantOrderNo (訂單編號) 必須跟原本的一樣！
                // 還有時間戳記 (TimeStamp) 必須是「現在的時間」，不能用舊的！
                var tradeParams = new Dictionary<string, string>
                {
                    { "MerchantID", merchantID },
                    { "RespondType", "JSON" },
                    { "TimeStamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() }, // ★ 關鍵：用新的時間
                    { "Version", "2.0" },
                    { "MerchantOrderNo", order.MerchantTradeNo }, // ★ 關鍵：用舊的訂單編號
                    { "Amt", ((int)order.FinalAmount).ToString() },     // 金額
                    { "ItemDesc", "PetSociety 補繳費用" },        // 商品描述
                    { "ReturnURL", returnUrl },
                    { "NotifyURL", notifyUrl },
                    { "Email", order.ReceiverEmail ?? "" },       // 如果訂單沒存 Email，就留空
                    { "LoginType", "0" },
                };

                // (C) 加密 (跟 CreateOrder 一樣)
                string paramString = NewebPayCrypto.ToQueryString(tradeParams);
                string tradeInfo = NewebPayCrypto.EncryptAESHex(paramString, hashKey, hashIV);
                string rawSha = $"HashKey={hashKey}&{tradeInfo}&HashIV={hashIV}";
                string tradeSha = NewebPayCrypto.EncryptSHA256(rawSha);

                // 4. 回傳給前端
                return Ok(new
                {
                    MerchantID = merchantID,
                    TradeInfo = tradeInfo,
                    TradeSha = tradeSha,
                    Version = "2.0",
                    ActionUrl = _configuration["NewebPay:ServiceUrl"] // 藍新的網址
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得付款資訊失敗: {ex.Message}");
            }
        }
        // PUT: api/orders/5/cancel
        // 取消訂單並恢復庫存
        [HttpPut("{orderId}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                int memberId = User.GetMemberId();
                
                // 呼叫剛剛寫好的 Service 方法
                await _orderService.CancelOrderAsync(orderId, memberId);

                return Ok(new { Message = "訂單已取消，庫存已恢復" });
            }
            catch (Exception ex)
            {
                // 如果是 Service 拋出的錯誤 (例如找不到訂單、狀態不對)，回傳 400
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("Callback")]
        public async Task<IActionResult> Callback([FromForm] IFormCollection form)
        {
            try
            {
                // 1. 接收藍新回傳的加密資料
                var tradeInfo = form["TradeInfo"];
                var tradeSha = form["TradeSha"];

                // 2. 準備解密鑰匙 (從 appsettings.json 拿)
                var hashKey = _configuration["NewebPay:HashKey"];
                var hashIV = _configuration["NewebPay:HashIV"];

                Console.WriteLine($"🔍 收到藍新回傳！開始解密...");

                // 3. 解密！(呼叫我們剛剛寫好的靜態工具)
                string decryptedJson = NewebPayCrypto.DecryptAES256(tradeInfo, hashKey, hashIV);

                Console.WriteLine($"🔓 解密結果: {decryptedJson}"); // 除錯用，之後可以拿掉

                // 4. 解析 JSON (把字串變成物件)
                // 藍新的格式是： { "Status": "SUCCESS", "Result": { ... } }
                dynamic resultObj = JsonConvert.DeserializeObject(decryptedJson);

                // 5. 判斷交易是否成功
                if (resultObj?.Status == "SUCCESS")
                {
                    // 6. 取出關鍵資料
                    string merchantOrderNo = resultObj.Result.MerchantOrderNo;
                    string tradeNo = resultObj.Result.TradeNo;
                    string payTime = resultObj.Result.PayTime;

                    // 7. 🔥 呼叫 Service 做正事！(改狀態、扣庫存)
                    await _orderService.HandlePaymentSuccessAsync(merchantOrderNo, tradeNo, payTime);

                    Console.WriteLine("✅ 訂單更新成功！");

                    // 8. 成功 -> 轉址回前端成功頁面
                    return Redirect("http://localhost:4200/mall/payment-success");
                }
                else
                {
                    // 失敗 -> 印出原因
                    Console.WriteLine($"❌ 付款失敗：{resultObj?.Message}");
                    // 轉址回購物車 (或是你有做失敗頁面也可以)
                    return Redirect("http://localhost:4200/mall/cart");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Callback 發生錯誤: {ex.Message}");
                // 發生例外也是先踢回首頁或購物車，避免使用者看到白畫面
                return Redirect("http://localhost:4200/mall");
            }

        }
        // DELETE: api/orders/5
        // 軟刪除訂單紀錄
        [HttpDelete("{orderId}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            try
            {
                int memberId = User.GetMemberId();
                await _orderService.DeleteOrderAsync(orderId, memberId);
                return Ok(new { Message = "訂單紀錄已刪除 (眼不見為淨)" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
