using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetSocietyWeb.Areas.Class.ViewModels
{
    public class CourseFormViewModel
    {
        public CourseViewModel Course { get; set; }
        public CourseDetailViewModel? Details { get; set; }

        public List<CourseChaptersViewModel> Chapters { get; set; } = new();

        

        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> AreaList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> InstructorList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> StatusList { get; set; }
    }
}
