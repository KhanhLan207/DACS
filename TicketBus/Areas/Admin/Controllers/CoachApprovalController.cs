using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TicketBus.Data;
using TicketBus.Models;

namespace TicketBus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CoachApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CoachApprovalController> _logger;

        public CoachApprovalController(ApplicationDbContext context, ILogger<CoachApprovalController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Admin/CoachApproval/Index
        public async Task<IActionResult> Index()
        {
            var coaches = await _context.Coaches
                .AsNoTracking()
                .Include(c => c.Brand)
                .Include(c => c.VehicleType)
                .Include(c => c.RegistForm)
                .OrderBy(c => c.IdRegist)
                .ToListAsync();

            return View(coaches);
        }

        // POST: /Admin/CoachApproval/ApproveCoach/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCoach(int id)
        {
            var coach = await _context.Coaches
                .Include(c => c.RegistForm)
                .Include(c => c.Brand)
                .Include(c => c.VehicleType)
                .FirstOrDefaultAsync(c => c.IdCoach == id);

            if (coach == null)
            {
                _logger.LogWarning("Coach with ID {Id} not found.", id);
                return Json(new { success = false, message = "Không tìm thấy xe." });
            }

            if (coach.State != CoachState.ChoPheDuyet)
            {
                _logger.LogWarning("Coach {CoachId} is not in ChoPheDuyet state.", id);
                return Json(new { success = false, message = $"Xe {coach.NumberPlate} không ở trạng thái chờ phê duyệt." });
            }

            try
            {
                coach.State = CoachState.HoatDong;
                if (coach.RegistForm != null)
                {
                    coach.RegistForm.State = RegistFormState.DaXuLy;
                }
                else
                {
                    _logger.LogWarning("RegistForm for Coach {CoachId} is null.", id);
                }

                _context.Update(coach);
                await _context.SaveChangesAsync();

                // Lưu thông báo cho quản lý hãng xe
                var notification = new Notification
                {
                    UserId = coach.Brand.UserId,
                    Message = $"Xe {coach.NumberPlate} đã được phê duyệt thành công.",
                    CreatedDate = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Coach {CoachId} approved successfully: NumberPlate={NumberPlate}, Brand={BrandName}",
                    id, coach.NumberPlate, coach.Brand?.NameBrand ?? "Unknown");
                return Json(new { success = true, message = $"Đã phê duyệt xe {coach.NumberPlate} ({coach.VehicleType?.NameType ?? "Unknown"}) thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to approve coach {CoachId}.", id);
                return Json(new { success = false, message = $"Có lỗi xảy ra khi phê duyệt xe {coach.NumberPlate}. Vui lòng thử lại." });
            }
        }

        // POST: /Admin/CoachApproval/RejectCoach/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCoach(int id, string rejectReason)
        {
            var coach = await _context.Coaches
                .Include(c => c.RegistForm)
                .Include(c => c.Brand)
                .Include(c => c.VehicleType)
                .FirstOrDefaultAsync(c => c.IdCoach == id);

            if (coach == null)
            {
                _logger.LogWarning("Coach with ID {Id} not found.", id);
                return Json(new { success = false, message = "Không tìm thấy xe." });
            }

            if (coach.State != CoachState.ChoPheDuyet)
            {
                _logger.LogWarning("Coach {CoachId} is not in ChoPheDuyet state.", id);
                return Json(new { success = false, message = $"Xe {coach.NumberPlate} không ở trạng thái chờ phê duyệt." });
            }

            if (string.IsNullOrEmpty(rejectReason))
            {
                return Json(new { success = false, message = "Vui lòng nhập lý do từ chối." });
            }

            try
            {
                coach.State = CoachState.KhongHoatDong;
                coach.RejectReason = rejectReason;
                if (coach.RegistForm != null)
                {
                    coach.RegistForm.State = RegistFormState.DaXuLy;
                }
                else
                {
                    _logger.LogWarning("RegistForm for Coach {CoachId} is null.", id);
                }

                _context.Update(coach);
                await _context.SaveChangesAsync();

                // Lưu thông báo cho quản lý hãng xe
                var notification = new Notification
                {
                    UserId = coach.Brand.UserId,
                    Message = $"Xe {coach.NumberPlate} đã bị từ chối. Lý do: {rejectReason}",
                    CreatedDate = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Coach {CoachId} rejected successfully: NumberPlate={NumberPlate}, Brand={BrandName}, Reason={RejectReason}",
                    id, coach.NumberPlate, coach.Brand?.NameBrand ?? "Unknown", rejectReason);
                return Json(new { success = true, message = $"Đã từ chối xe {coach.NumberPlate} ({coach.VehicleType?.NameType ?? "Unknown"}) thành công! Lý do: {rejectReason}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reject coach {CoachId}.", id);
                return Json(new { success = false, message = $"Có lỗi xảy ra khi từ chối xe {coach.NumberPlate}. Vui lòng thử lại." });
            }
        }
    }
}