using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketBus.Data;
using TicketBus.Models;
using Microsoft.Extensions.Logging;

namespace TicketBus.Areas.Brand.Controllers
{
    [Area("Brand")]
    [Authorize(Roles = "Brand")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Không thể xác định người dùng. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var brand = await _context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.UserId == userId && b.State == BrandState.HoatDong);

            if (brand == null)
            {
                TempData["Message"] = "Hãng xe của bạn chưa được phê duyệt hoặc không tồn tại.";
            }

            ViewBag.BrandInfo = brand;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovedCoaches()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { coaches = new List<object>() });
            }

            var brand = await _context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.UserId == userId && b.State == BrandState.HoatDong);

            if (brand == null)
            {
                return Json(new { coaches = new List<object>() });
            }

            var coaches = await _context.Coaches
                .Where(c => c.IdBrand == brand.IdBrand && c.State == CoachState.DaPheDuyet)
                .Include(c => c.VehicleType)
                .Select(c => new
                {
                    coachCode = c.CoachCode,
                    numberPlate = c.NumberPlate,
                    vehicleType = c.VehicleType != null ? c.VehicleType.NameType : null,
                    state = (int)c.State
                })
                .ToListAsync();

            return Json(new { coaches });
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { unreadCount = 0, notifications = new List<object>() });
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .Take(10)
                .Select(n => new
                {
                    id = n.Id,
                    message = n.Message,
                    createdDate = n.CreatedDate.ToString("o"),
                    isRead = n.IsRead
                })
                .ToListAsync();

            var unreadCount = notifications.Count(n => !n.isRead);

            return Json(new { unreadCount, notifications });
        }

        public IActionResult GoToHomePage()
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("MarkNotificationAsRead: UserId is null or empty.");
                return Json(new { success = false, message = "Không thể xác định người dùng." });
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (notification == null)
            {
                _logger.LogWarning("MarkNotificationAsRead: Notification with ID {Id} not found for UserId {UserId}.", id, userId);
                return Json(new { success = false, message = "Thông báo không tồn tại." });
            }

            try
            {
                notification.IsRead = true;
                _context.Update(notification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("MarkNotificationAsRead: Notification {Id} marked as read for UserId {UserId}.", id, userId);
                return Json(new { success = true, message = "Đã đánh dấu thông báo là đã đọc." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MarkNotificationAsRead: Failed to mark notification {Id} as read for UserId {UserId}.", id, userId);
                return Json(new { success = false, message = "Có lỗi xảy ra khi đánh dấu thông báo. Vui lòng thử lại." });
            }
        }
    }
}