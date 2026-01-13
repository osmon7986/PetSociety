using System.Net;
using System.Net.Mail;

namespace PetSociety.API.Helpers
{

    public class EmailHelper
    {
        // 設定你的 Gmail 帳號與應用程式密碼
        // 注意：這裡不能用一般的 Gmail 密碼，要去 Google 帳號設定申請「應用程式密碼 (App Password)」
        private readonly string _senderEmail = "petsociety.usertest@gmail.com";
        private readonly string _senderPassword = "dfrvvtfaqnpxlpqt";

        public void SendPasswordResetEmail(string toEmail, string token)
        {
            var fromAddress = new MailAddress(_senderEmail, "PetSociety 官方");
            var toAddress = new MailAddress(toEmail);

            // 設定信件標題與內容
            const string subject = "【PetSociety】重設密碼驗證信";

            // 這裡假設你的前端重設密碼頁面網址是 localhost:4200/reset-password
            string body = $@"
                <h2>重設密碼請求</h2>
                <p>我們收到了您重設密碼的請求。</p>
                <p>請使用以下驗證碼進行重設 (10分鐘內有效)：</p>
                <h1 style='color: #D8C3A5;'>{token}</h1>
                <p>或是點擊連結：<a href='http://localhost:4200/reset-password?token={token}&email={toEmail}'>重設密碼</a></p>
            ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, _senderPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true // 支援 HTML 格式
            })
            {
                smtp.Send(message);
            }
        }

        public void SendAccountVerificationEmail(string toEmail, string code)
        {
            var fromAddress = new MailAddress(_senderEmail, "PetSociety 官方");
            var toAddress = new MailAddress(toEmail);

            const string subject = "【PetSociety】會員註冊驗證碼";

            string body = $@"
                    <div style='font-family: Arial, sans-serif; color: #333;'>
                        <h2>歡迎加入 PetSociety！</h2>
                        <p>感謝您的註冊，請使用以下驗證碼啟用您的帳號：</p>
                        <h1 style='color: #D8C3A5; letter-spacing: 5px;'>{code}</h1>
                        <p>如果您沒有註冊本網站，請忽略此信件。</p>
                    </div>
                ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, _senderPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}