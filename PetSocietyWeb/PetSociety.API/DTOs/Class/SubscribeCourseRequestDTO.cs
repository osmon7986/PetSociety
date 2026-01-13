using System.ComponentModel.DataAnnotations;

namespace PetSociety.API.DTOs.Class
{
    public class SubscribeCourseRequestDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CourseDetailId { get; set; }
    }
}
