using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetSocietyWeb.Areas.Class.Models.Enum;

namespace PetSocietyWeb.Areas.Class.ViewModels
{
    public class CourseDetailViewModel
    {
        public string? ImageUrl { get; set; }

        public int CourseDetailId { get; set; }

        [Display(Name = "所屬區域")]
        public byte? AreaId { get; set; }

        [Display(Name = "講師")]
        public int? InstructorId { get; set; }

        [DisplayFormat(DataFormatString ="{0:N0}", ApplyFormatInEditMode = false)]
        [Display(Name = "課程價格")]
        public decimal? Price { get; set; }

        [Required]
        [Display(Name = "發布狀態")]
        public CourseStatus Status { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "開始日期")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "結束日期")]
        public DateTime? EndDate { get; set; }

        public DateTime? CreatedDate { get; set; }
        public List<CourseChaptersViewModel> Chapters { get; set; } = new();

        public IFormFile? ImageFile { get; set; }
    }
}
