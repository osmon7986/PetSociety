using System.ComponentModel.DataAnnotations;

namespace PetSociety.API.DTOs.Class
{
    public class UpdateChapterRecordDTO
    {
        [Required]
        public int CourseDetailId { get; set; }
        [Required]
        public int ChapterId { get; set; }
        
    }
}
