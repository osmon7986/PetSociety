using System.ComponentModel.DataAnnotations;
using PetSocietyWeb.Models.Generated;

namespace PetSocietyWeb.Areas.Class.ViewModels
{
    public class ChapterViedoViewModel
    {
        public int VideoId { get; set; }
        public string? FullVideoUrl { get; set; }

        [Display(Name = "章節影片")]
        public string? VideoUrl { get; set; }

        
    }
}
