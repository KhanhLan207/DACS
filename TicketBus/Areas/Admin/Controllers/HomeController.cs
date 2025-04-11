using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketBus.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")] // Chỉ Admin truy cập được
        public IActionResult AdminPanel()
        {
            return View();
        }

        [Authorize(Roles = "NhanVien")] // Chỉ Nhân Viên truy cập được
        public IActionResult EmployeePanel()
        {
            return View();
        }

        [Authorize(Roles = "Admin, NhanVien")] // Cả hai vai trò đều truy cập được
        public IActionResult GeneralPanel()
        {
            return View();
        }
    }
}
