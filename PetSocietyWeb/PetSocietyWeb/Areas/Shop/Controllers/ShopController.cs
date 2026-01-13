using Microsoft.AspNetCore.Mvc;

namespace PetSocietyWeb.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
