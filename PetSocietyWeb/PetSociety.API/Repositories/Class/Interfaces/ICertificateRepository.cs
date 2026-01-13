using PetSociety.API.Models;

namespace PetSociety.API.Repositories.Class.Interfaces
{
    public interface ICertificateRepository
    {
        Task<UserCertificate?> GetByIds(int memberId, int courseDetailId);
        Task CreateCertificate(UserCertificate model);
    }
}
