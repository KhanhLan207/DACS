using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketBus.Data;
using TicketBus.Models;

namespace TicketBus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SchedulePriceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SchedulePriceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/SchedulePrice/Index
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách lịch trình kèm thông tin tuyến đường và xe
            var schedules = await _context.ScheduleDetails
                .Include(s => s.BusRoute)
                .Include(s => s.Coach)
                .Include(s => s.Prices)
                    .ThenInclude(p => p.RouteStopStart)
                .Include(s => s.Prices)
                    .ThenInclude(p => p.RouteStopEnd)
                .ToListAsync();

            return View(schedules);
        }
    }
}