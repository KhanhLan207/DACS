using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketBus.Areas.Brand.Controllers
{
    [Area("Brand")]
    [Authorize(Roles = "Brand")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GoToHomePage()
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
