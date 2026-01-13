using Microsoft.AspNetCore.Mvc.Rendering;
using PetSocietyWeb.Areas.Class.Extensions;
using PetSocietyWeb.Areas.Class.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace PetSocietyWeb.Areas.Class.ViewModels
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }
        [Required]
        public int CourseDetailId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "課程名稱必填")]
        [Display(Name = "課程名稱")]
        public string Title { get; set; }

        [Display(Name = "課程描述")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "課程分類必填")]
        [Display(Name = "課程分類")]
        public byte CategoryId { get; set; }

        [Display(Name = "實體課")]
        public bool Type { get; set; }
        // Enum
        public CourseStatus Status { get; set; }
        public string StatusName => Status.GetDisplayName();
        public DateTime? CreatedDate { get; set; }
    }
}
