using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketBus.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            // Chuyển hướng dựa trên vai trò
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminPanel");
            }
            else if (User.IsInRole("NhanVien"))
            {
                return RedirectToAction("EmployeePanel");
            }
            else
            {
                // Nếu không có vai trò phù hợp, chuyển về trang chủ hoặc hiển thị lỗi
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

            [Authorize(Roles = "Admin")]
            public IActionResult AdminPanel()
            {
                return View();
            }

            [Authorize(Roles = "NhanVien")]
            public IActionResult EmployeePanel()
            {
                return View();
            }

            [Authorize(Roles = "Admin, NhanVien")]
            public IActionResult GeneralPanel()
            {
                return View();
            }
    }
}
