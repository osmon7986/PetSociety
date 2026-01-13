using PetSociety.API.DTOs.Class;
using PetSociety.API.Exceptions;
using PetSociety.API.Models;
using PetSociety.API.Repositories.Class.Interfaces;
using PetSociety.API.Repositories.User.Interfaces;
using PetSociety.API.Services.Class.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace PetSociety.API.Services.Class.Implementations
{
    public class CertificateService : ICertificateService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IWebHostEnvironment _env;

        public CertificateService(
            IMemberRepository memberRepository, 
            ICertificateRepository certificateRepository,
            IWebHostEnvironment env)
        {
            _memberRepository = memberRepository;
            _certificateRepository = certificateRepository;
            _env = env;
        }

        

        // 建立 QuestPDF 模板
        public byte[] BuildCertificatePdf(CourseCertificateDTO model)
        {
            // 印鑑章路徑
            var sealPath = Path.Combine(_env.WebRootPath, "img", "Seal.png");
            // 證書背景路徑
            var bgPath = Path.Combine(_env.WebRootPath, "img", "background.png");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape()); // A4 橫向
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontFamily("Times New Roman"));
                    page.Background()
                    .Image(bgPath)
                    .FitArea();

                    page.Content().Row(row =>
                    {
                        row.RelativeItem()
                            .AlignCenter()
                            .Column(col =>
                            {
                                col.Spacing(14);

                                // 證書標頭
                                col.Item().AlignCenter().PaddingTop(30).Text("PetSociety Academy Certificate")
                                    .FontSize(14)
                                    .FontColor(Colors.Grey.Darken1);
                                // 主標語
                                col.Item().AlignCenter().PaddingTop(30).Text("PetSociety 學院 謹此證明")
                                    .FontSize(22);
                                // 姓名
                                col.Item().AlignCenter().PaddingTop(50).Text(model.MemberName)
                                    .FontSize(40)
                                    .Bold();
                                // 課程說明
                                col.Item().AlignCenter().PaddingTop(15).Text($"已完成 {model.CourseTitle} 課程之所有學習內容")
                                    .FontSize(14);
                                // 日期
                                col.Item().AlignCenter().PaddingTop(20).Text(
                                     $"頒發日期： {model.CompletedDate:yyyy年MM月dd日}") // {變數:格式} 套用右邊日期格式
                                    .FontSize(12);
                                
                                col.Item().AlignCenter().PaddingTop(10).Row(r =>
                                {
                                    r.RelativeItem().Column(c =>
                                    {
                                        // 印章
                                        if (File.Exists(sealPath))
                                        {
                                            c.Item()
                                            .Height(120)
                                            .AlignCenter()
                                            .Image(sealPath)
                                            .FitArea();
                                        }
                                    
                                        c.Item().AlignCenter().Text("PetSociety 學院")
                                            .FontSize(10);
                                    });
                                });
                            });
                        
                    });

                    
                });
            });

            return document.GeneratePdf();
        }

        // 讓 controller 呼叫的方法
        public byte[] GenerateCertificatePdf(CourseCertificateDTO model)
        {
            return BuildCertificatePdf(model);
        }

        public async Task<CourseCertificateDTO?> GetCertificateData(int memberId, int courseDetailId)
        {

            // 查詢證書資料
            var certificate = await _certificateRepository.GetByIds(memberId, courseDetailId);
            if (certificate == null)
            {
                return null;
            }


            return new CourseCertificateDTO
            {
                MemberName = certificate.Member.Name,
                CourseTitle = certificate.CourseDetail.Course.Title,
                CompletedDate = certificate.Member.UserCourseRecords?.FirstOrDefault()?.CompletedAt ??
                                throw new BusinessException("課程尚未完成，無法生產證書"),
            };

        }

        public async Task IssueUserCertificate(int memberId, int courseDetailId)
        {
            var certificate = new UserCertificate
            {
                MemberId = memberId,
                CourseDetailId = courseDetailId,
            };
            await _certificateRepository.CreateCertificate(certificate);
        }
    }
}
