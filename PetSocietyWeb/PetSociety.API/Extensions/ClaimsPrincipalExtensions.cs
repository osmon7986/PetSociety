
using System.Security.Claims;

namespace PetSociety.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Get memberId from token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static int GetMemberId(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(id, out int memberId))
                throw new UnauthorizedAccessException("無效的會員ID");

            return memberId;
        }
    }
}
