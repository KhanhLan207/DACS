using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketBus.Areas.NhanVien.Controllers
{
    public class HomeController : Controller
    {
        [Area("NhanVien")]
        [Authorize(Roles = "NhanVien")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
