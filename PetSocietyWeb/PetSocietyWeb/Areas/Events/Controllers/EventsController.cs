using Microsoft.AspNetCore.Mvc;

namespace PetSocietyWeb.Areas.Events.Controllers
{
    [Area("Events")]
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
