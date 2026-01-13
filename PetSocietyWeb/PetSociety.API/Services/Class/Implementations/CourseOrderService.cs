using PetSociety.API.Enums.Course;
using PetSociety.API.Exceptions;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;
using PetSociety.API.Services.Class.Interfaces;

namespace PetSociety.API.Services.Class.Implementations
{
    public class CourseOrderService : ICourseOrderService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly PetSocietyContext _context;
        public CourseOrderService(ICourseRepository courseRepository, PetSocietyContext context)
        {
            _courseRepository = courseRepository;
            _context = context;
        }

        public async Task<CourseOrder> CreateOrderAsync(int memberId, int courseDetailId)
        {
            // 與資料庫核對是否有此課程，並且取得結帳所需課程資料
            var course = await _courseRepository.GetForCheckOutAsync(courseDetailId) ?? throw new Exception("找不到此課程"); 
            // 產生唯一的訂單編號
            string tradeNo = $"PS{DateTime.Now:yyyyMMddHHmmss}{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

            var order = new CourseOrder
            {
                CourseDetailId = courseDetailId,
                CourseTitle = course.Course.Title,
                BuyerMemberId = memberId,
                MerchantTradeNo = tradeNo,
                IsGift = false,
                Amount = course.Price,
                OrderStatus = (int)OrderStatus.Unpaid,
                IsPaid = false,
                OrderedDate = DateTime.Now
            };
            // 將訂單資料存入資料庫
            await _courseRepository.AddCourseOrder(order);

            return order;
        }

        public async Task UpdateOrderToPaidAsync(string merchantTradeNo, string ecpayTradeNo)
        {
            var order = await _courseRepository.GetOrderbyTradeNoAsync(merchantTradeNo);
            // 如果沒有資料就return
            if (order == null)
            {
                return;
            }
            // 檢查付款狀態，已付款就不更新
            if (order.IsPaid || order.OrderStatus == (int)OrderStatus.Paid)
            {
                return;
            }
            // 更新訂單狀態和付款日期，紀錄綠界交易編號
            order.OrderStatus = (int)OrderStatus.Paid;
            order.IsPaid = true;
            order.PaymentDate = DateTime.Now;
            order.TransactionId = ecpayTradeNo;

            await _context.SaveChangesAsync();
        }
    }
}
