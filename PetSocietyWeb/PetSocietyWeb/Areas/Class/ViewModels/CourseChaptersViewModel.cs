using System.ComponentModel.DataAnnotations;

namespace PetSocietyWeb.Areas.Class.ViewModels
{
    public class CourseChaptersViewModel
    {
        public int ChapterId { get; set; }

        public int CourseDetailId { get; set; }

        [Display(Name = "章節標題")]
        public string ChapterName { get; set; }

        [Display(Name = "章節大綱")]
        public string? Summary { get; set; }

        public int SortOrder { get; set; }

        public int? Duration { get; set; }

        public ChapterViedoViewModel Video { get; set; } = new();
    }
}
