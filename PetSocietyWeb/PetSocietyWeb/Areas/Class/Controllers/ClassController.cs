using Microsoft.AspNetCore.Mvc;

namespace PetSocietyWeb.Areas.Class.Controllers
{
    [Area("Class")]
    public class ClassController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
