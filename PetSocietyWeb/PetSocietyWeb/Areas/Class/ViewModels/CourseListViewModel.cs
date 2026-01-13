using PetSocietyWeb.Areas.Class.Models.Enum;

namespace PetSocietyWeb.Areas.Class.ViewModels
{
    public class CourseListViewModel
    {
        public int CourseId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public byte CategoryId { get; set; }

        public bool Type { get; set; }
        public int CourseDetailId { get; set; }
        public string CategoryName { get; set; }

        public byte? AreaId { get; set; }

        public int? InstructorId { get; set; }
        public string Name { get; set; }

        public decimal? Price { get; set; }

        public CourseStatus Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public string ImageUrl { get; set; }

    }
}
