using System.ComponentModel.DataAnnotations;

namespace PetSociety.API.DTOs.Class
{
    public class CourseCertificateDTO
    {

        public string MemberName { get; set; }

        public string CourseTitle { get; set; }
        
        public DateTime? CompletedDate { get; set; }
    }
}
