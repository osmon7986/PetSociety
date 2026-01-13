using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PetSociety.API.DTOs.Class;
using PetSociety.API.Extensions;
using PetSociety.API.Services.Class.Interfaces;
using System.Security.Claims;

namespace PetSociety.API.Controllers.Class
{
    [Route("[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        public CertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        // 取得會員證書資料
        // GET: Certificate
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<CourseCertificateDTO>> GetCertificateData(int courseDetailId)
        {
            var memberId = User.GetMemberId();

            var result = await _certificateService.GetCertificateData(memberId, courseDetailId);

            if (result == null)
            {
                return NotFound(); // 404
            }

            return Ok(result); // 200
        }

        // 取得會員證書 PDF
        // GET: Certificate/courseDetailId/pdf
        [HttpGet("{courseDetailId}/pdf")]
        [Authorize]
        public async Task<ActionResult> DownloadCertificatePdf(int courseDetailId)
        {
            var memberId = User.GetMemberId();
            
            var result = await _certificateService.GetCertificateData(memberId, courseDetailId);

            if (result == null)
            {
                return NotFound(); // 404
            }

            var pdfBytes = _certificateService.GenerateCertificatePdf(result);

            return File(pdfBytes, "application/pdf", "certificate.pdf");
        }
        

    }
}
