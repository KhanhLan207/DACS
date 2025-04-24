using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketBus.Data;
using TicketBus.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TicketBus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CoachController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CoachController> _logger;

        public CoachController(ApplicationDbContext context, ILogger<CoachController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Admin/Coach/PendingApproval
        public async Task<IActionResult> PendingApproval(string filter = "pending")
        {
            IQueryable<Coach> coachesQuery = _context.Coaches
                .Include(c => c.VehicleType)
                .Include(c => c.Brand);

            switch (filter)
            {
                case "approved":
                    coachesQuery = coachesQuery.Where(c => c.State == CoachState.DaPheDuyet);
                    break;
                case "rejected":
                    coachesQuery = coachesQuery.Where(c => c.State == CoachState.TuChoi);
                    break;
                case "pending":
                default:
                    coachesQuery = coachesQuery.Where(c => c.State == CoachState.ChoPheDuyet);
                    break;
            }

            var coaches = await coachesQuery.ToListAsync();
            ViewBag.Filter = filter;

            return View(coaches);
        }

        // POST: /Admin/Coach/Approve/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var coach = await _context.Coaches
                .Include(c => c.Brand)
                .FirstOrDefaultAsync(c => c.IdCoach == id && c.State == CoachState.ChoPheDuyet);

            if (coach == null)
            {
                _logger.LogWarning("Approve: Coach with Id {IdCoach} not found or not in pending state.", id);
                return Json(new { success = false, message = "Không tìm thấy xe hoặc xe không ở trạng thái chờ phê duyệt." });
            }

            try
            {
                coach.State = CoachState.DaPheDuyet;
                _context.Coaches.Update(coach);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Approve: Coach {CoachCode} has been approved.", coach.CoachCode);

                // Tạo thông báo cho hãng xe
                var notification = new Notification
                {
                    UserId = coach.Brand.UserId,
                    Message = $"Xe {coach.CoachCode} ({coach.NumberPlate}) đã được phê duyệt.",
                    CreatedDate = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Phê duyệt xe thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Approve: Failed to approve Coach {CoachCode}. Error: {Error}", coach.CoachCode, ex.Message);
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // POST: /Admin/Coach/Reject/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string rejectReason)
        {
            var coach = await _context.Coaches
                .Include(c => c.Brand)
                .FirstOrDefaultAsync(c => c.IdCoach == id && c.State == CoachState.ChoPheDuyet);

            if (coach == null)
            {
                _logger.LogWarning("Reject: Coach with Id {IdCoach} not found or not in pending state.", id);
                return Json(new { success = false, message = "Không tìm thấy xe hoặc xe không ở trạng thái chờ phê duyệt." });
            }

            try
            {
                coach.State = CoachState.TuChoi;
                _context.Coaches.Update(coach);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Reject: Coach {CoachCode} has been rejected.", coach.CoachCode);

                // Tạo thông báo cho hãng xe
                var notification = new Notification
                {
                    UserId = coach.Brand.UserId,
                    Message = $"Xe {coach.CoachCode} ({coach.NumberPlate}) đã bị từ chối. Lý do: {rejectReason}",
                    CreatedDate = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Từ chối xe thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reject: Failed to reject Coach {CoachCode}. Error: {Error}", coach.CoachCode, ex.Message);
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // GET: /Admin/Coach/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var coach = await _context.Coaches
                .Include(c => c.VehicleType)
                .Include(c => c.Brand)
                .FirstOrDefaultAsync(c => c.IdCoach == id);

            if (coach == null)
            {
                return NotFound("Không tìm thấy xe.");
            }

            // Deserialize JSON thành danh sách
            var images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(coach.Images) ?? new List<string>();
            var documents = System.Text.Json.JsonSerializer.Deserialize<List<string>>(coach.Documents) ?? new List<string>();

            ViewBag.Images = images;
            ViewBag.Documents = documents;

            return View(coach);
        }
    }
}