using Microsoft.AspNetCore.Mvc;

namespace PetSocietyWeb.Areas.Community.Controllers
{
    [Area("Community")]
    public class CommunityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
