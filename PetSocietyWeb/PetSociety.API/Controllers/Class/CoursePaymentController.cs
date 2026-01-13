using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSociety.API.DTOs.Class;
using PetSociety.API.Exceptions;
using PetSociety.API.Extensions;
using PetSociety.API.Repositories.Class.Interfaces;
using PetSociety.API.Services.Class.Interfaces;

namespace PetSociety.API.Controllers.Class
{
    [Route("[controller]")]
    [ApiController]
    public class CoursePaymentController : ControllerBase
    {
        private readonly IEcpayPaymentService _ecpayPaymentService;
        private readonly ICourseOrderService _courseOrderService;
        private readonly ICourseRepository _courseRepository;
        public CoursePaymentController(IEcpayPaymentService ecpayPaymentService, 
            ICourseOrderService courseOrderService,
            ICourseRepository courseRepository)
        {
            _ecpayPaymentService = ecpayPaymentService;
            _courseOrderService = courseOrderService;
            _courseRepository = courseRepository;
        }

        // POST: CoursePayment/checkout
        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDTO request)
        {
            try
            {
                var memberId = User.GetMemberId();
                var newCourseOrder = await _courseOrderService.CreateOrderAsync(memberId, request.CourseDetailId);
                // 產生綠界付款所需參數
                EcpayCheckOutDTO dto = new EcpayCheckOutDTO
                {
                    Amount = (int)newCourseOrder.Amount,
                    ItemName = newCourseOrder.CourseTitle,
                    OrderId = newCourseOrder.MerchantTradeNo,
                    ClientBackUrl = request.clientBackUrl
                };

                var param = _ecpayPaymentService.GetPaymentParameters(dto);

                return Ok(param); // 測試
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        // 綠界付款完成後的callback
        // POST: CoursePayment/callback
        [IgnoreAntiforgeryToken]
        [HttpPost("callback")]
        [Consumes("application/x-www-form-urlencoded")] // 綠界用表單格式回傳
        public async Task<IActionResult> Callback([FromForm]IFormCollection collection)
        {
            // 把綠界回傳的form collection 轉成字典
            var dict = collection.ToDictionary(x => x.Key, x => x.Value.ToString());

            if (dict["RtnCode"] == "1") // 確認付款是否成功
            {
                string merchTradeNo = dict["MerchantTradeNo"]; // 拿我們的訂單編號
                string transactionNo = dict["TradeNo"]; // 拿綠界的交易編號
                await _courseOrderService.UpdateOrderToPaidAsync(merchTradeNo, transactionNo);
            }

            return Content("1|OK"); // 綠界要求的回應格式
        }

        // 檢查會員此課程購買紀錄是否存在且有效
        // GET: CoursePayment/checkOwnerShip/courseDetailId
        [Authorize]
        [HttpGet("checkOwnerShip/{courseDetailId}")]
        public async Task<bool> CheckOwnerShip(int courseDetailId)
        {
            var memberId = User.GetMemberId();
            return await _courseRepository.HasPurchased(memberId, courseDetailId);
        }
    }
}
