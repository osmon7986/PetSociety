using System.ComponentModel.DataAnnotations;

namespace PetSocietyWeb.Areas.Class.Models.Enum
{
    public enum CourseStatus:byte
    {
        [Display(Name = "草稿")]
        Draft = 0,

        [Display(Name = "上架")]
        Active = 1,

        [Display(Name = "下架")]
        Disabled = 2
    }
}
