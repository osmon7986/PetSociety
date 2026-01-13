using Microsoft.Extensions.Options;
using PetSociety.API.DTOs.Class;
using PetSociety.API.Models;
using PetSociety.API.Services.Class.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PetSociety.API.Services.Class.Implementations
{
    public class EcpayPaymentService : IEcpayPaymentService
    {
        private readonly EcpaySettings _settings;
        public EcpayPaymentService(IOptions<EcpaySettings> settings) // IOptions<T> 強型別的寫法
        {
            _settings = settings.Value;
        }

        public string GenerateCheckMacValue(Dictionary<string, string> param) // 檢查碼(防偽印章)
        {
            // 按照第一個英文字母A到Z排序參數
            var sortedParams = param
                .Where(p => p.Key != "CheckMacValue")
                .OrderBy(p => p.Key)
                .Select(p => $"{p.Key}={p.Value}"); // 把字典參數用字串接起來 > IEnumerable<string>

            string stringDate = $"HashKey={_settings.HashKey}&{string.Join("&", sortedParams)}&HashIV={_settings.HashIV}"; // 把params串成一個長字串

            string encodeDate = HttpUtility.UrlEncode(stringDate).ToLower(); // 進行 UrlEncode + 轉小寫

            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(encodeDate)); // 用 SHA256 進行雜湊運算

            return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper(); // 把雜湊後的 byte 陣列轉成大寫十六進位字串
        }

        public Dictionary<string, string> GetPaymentParameters(EcpayCheckOutDTO dto)
        {
            string finalUrl = dto.ClientBackUrl ?? _settings.ClientBackURL; // 如果沒有帶回傳網址就用預設的
            string encodedUrl = Uri.EscapeDataString(finalUrl); // 進行 URL 編碼
            string successPage = $"http://localhost:4200/academy/payment/success?returnUrl={encodedUrl}"; 
            // 綠界需要的參數裝進字典
            var parameters = new Dictionary<string, string>
            {
                {"MerchantID", _settings.MerchantID },
                {"MerchantTradeNo", dto.OrderId },
                {"MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
                {"PaymentType", "aio" },
                {"TotalAmount", dto.Amount.ToString() },
                {"TradeDesc", HttpUtility.UrlEncode("PetSociety 學院課程訂單") }, // Transaction Description
                {"ItemName", dto.ItemName },
                {"ReturnURL", _settings.ReturnURL },
                {"ChoosePayment", "ALL" },
                {"EncryptType", "1" }, // 固定用 1，SHA256 加密
                {"ClientBackURL", successPage },
                {"CheckMacValue", "" }
            };
            // 計算檢查碼
            parameters["CheckMacValue"] = GenerateCheckMacValue(parameters);

            return parameters;
        }

    }
}
