using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TicketBus.Data;
using TicketBus.Models;

namespace TicketBus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BusRouteApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BusRouteApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/BusRouteApproval/PendingApproval
        public async Task<IActionResult> PendingApproval(string filter = "pending")
        {
            // Thiết lập giá trị filter mặc định là "pending" (Chờ phê duyệt)
            ViewBag.Filter = filter;

            // Truy vấn cơ bản, khai báo rõ ràng là IQueryable<BusRoute>
            IQueryable<BusRoute> query = _context.BusRoutes
                .AsNoTracking()
                .Include(r => r.Brand)
                .Include(r => r.StartCity)
                .Include(r => r.EndCity);

            // Lọc theo trạng thái
            switch (filter)
            {
                case "approved":
                    query = query.Where(r => r.State == BusRouteState.DaPheDuyet);
                    break;
                case "rejected":
                    query = query.Where(r => r.State == BusRouteState.TuChoi);
                    break;
                case "pending":
                default:
                    query = query.Where(r => r.State == BusRouteState.ChoPheDuyet);
                    break;
            }

            // Sắp xếp và lấy danh sách
            var pendingRoutes = await query.ToListAsync();

            return View(pendingRoutes);
        }

        // GET: /Admin/BusRouteApproval/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var route = await _context.BusRoutes
                .Include(r => r.Brand)
                .Include(r => r.StartCity)
                .Include(r => r.EndCity)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.City)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.District)
                .FirstOrDefaultAsync(r => r.IdRoute == id);

            if (route == null)
            {
                return NotFound();
            }

            return View(route);
        }

        // POST: /Admin/BusRouteApproval/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string reason)
        {
            var route = await _context.BusRoutes
                .Include(r => r.Brand)
                .FirstOrDefaultAsync(r => r.IdRoute == id);

            if (route == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái tuyến xe
            route.State = BusRouteState.DaPheDuyet;
            _context.Update(route);

            // Tạo thông báo cho Brand
            if (route.Brand?.UserId != null)
            {
                var notification = new Notification
                {
                    UserId = route.Brand.UserId,
                    Message = $"Tuyến xe '{route.NameRoute}' đã được phê duyệt. " +
                              (string.IsNullOrEmpty(reason) ? "" : $"Lý do: {reason}"),
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tuyến xe đã được phê duyệt thành công.";
            return RedirectToAction(nameof(PendingApproval));
        }

        // POST: /Admin/BusRouteApproval/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var route = await _context.BusRoutes
                .Include(r => r.Brand)
                .FirstOrDefaultAsync(r => r.IdRoute == id);

            if (route == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái tuyến xe
            route.State = BusRouteState.TuChoi;
            _context.Update(route);

            // Tạo thông báo cho Brand
            if (route.Brand?.UserId != null)
            {
                var notification = new Notification
                {
                    UserId = route.Brand.UserId,
                    Message = $"Tuyến xe '{route.NameRoute}' đã bị từ chối. " +
                              (string.IsNullOrEmpty(reason) ? "" : $"Lý do: {reason}"),
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tuyến xe đã bị từ chối thành công.";
            return RedirectToAction(nameof(PendingApproval));
        }
    }
}