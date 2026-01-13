using PetSociety.API.DTOs.Class;

namespace PetSociety.API.Services.Class.Interfaces
{
    public interface ICertificateService
    {
        /// <summary>
        /// Get member's certificate data
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="courseDetailId"></param>
        /// <returns></returns>
        Task<CourseCertificateDTO?> GetCertificateData(int memberId, int courseDetailId);
        Task IssueUserCertificate(int memberId, int courseDetailId);
        /// <summary>
        /// Build Certificate format
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        byte[] BuildCertificatePdf(CourseCertificateDTO model);
        /// <summary>
        /// Generate Certificate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        byte[] GenerateCertificatePdf(CourseCertificateDTO model);
    }
}
