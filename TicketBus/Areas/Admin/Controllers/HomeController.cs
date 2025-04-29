using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketBus.Data;
using TicketBus.Models;

namespace TicketBus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Home/AdminPanel
        public async Task<IActionResult> AdminPanel()
        {
            // Lấy số lượng hãng xe đang chờ phê duyệt
            var pendingBrandsCount = await _context.Brands
                .CountAsync(b => b.State == BrandState.ChoPheDuyet);

            // Lấy số lượng tuyến xe đang chờ phê duyệt
            var pendingBusRoutesCount = await _context.BusRoutes
                .CountAsync(r => r.State == BusRouteState.ChoPheDuyet);

            // Lấy số lượng xe đang chờ phê duyệt
            var pendingCoachesCount = await _context.Coaches
                .CountAsync(c => c.State == CoachState.ChoPheDuyet);

            // Lấy danh sách thông báo mới nhất (giới hạn 5 thông báo)
            var notifications = await _context.Notifications
                .OrderByDescending(n => n.CreatedDate)
                .Take(5)
                .ToListAsync();

            // **Dữ liệu cho Area Chart 1: Số tuyến xe theo trạng thái**
            var busRoutesByState = await _context.BusRoutes
                .GroupBy(r => r.State)
                .Select(g => new
                {
                    State = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var labelsAreaChart1 = new List<string> { "Chờ phê duyệt", "Đã phê duyệt", "Từ chối" };
            var dataAreaChart1 = new List<int>
            {
                busRoutesByState.FirstOrDefault(s => s.State == BusRouteState.ChoPheDuyet)?.Count ?? 0,
                busRoutesByState.FirstOrDefault(s => s.State == BusRouteState.DaPheDuyet)?.Count ?? 0,
                busRoutesByState.FirstOrDefault(s => s.State == BusRouteState.TuChoi)?.Count ?? 0
            };

            // **Dữ liệu cho Area Chart 2: Số thông báo theo tháng (6 tháng gần nhất)**
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var notificationsByMonth = await _context.Notifications
                .Where(n => n.CreatedDate >= sixMonthsAgo)
                .GroupBy(n => new { n.CreatedDate.Year, n.CreatedDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            var labelsAreaChart2 = new List<string>();
            var dataAreaChart2 = new List<int>();
            var currentDate = sixMonthsAgo;
            while (currentDate <= DateTime.UtcNow)
            {
                labelsAreaChart2.Add(currentDate.ToString("MM/yyyy"));
                var matchingMonth = notificationsByMonth
                    .FirstOrDefault(m => m.Year == currentDate.Year && m.Month == currentDate.Month);
                dataAreaChart2.Add(matchingMonth?.Count ?? 0);
                currentDate = currentDate.AddMonths(1);
            }

            // **Dữ liệu cho Bar Chart: Số xe theo hãng xe**
            var coachesByBrand = await _context.Coaches
                .Include(c => c.Brand)
                .GroupBy(c => c.Brand)
                .Select(g => new
                {
                    BrandName = g.Key != null ? g.Key.NameBrand : "Không có hãng",
                    Count = g.Count()
                })
                .OrderBy(g => g.BrandName)
                .ToListAsync();

            var labelsBarChart = coachesByBrand.Select(b => b.BrandName).ToList();
            var dataBarChart = coachesByBrand.Select(b => b.Count).ToList();

            // **Dữ liệu cho Pie Chart: Tỷ lệ thông báo đã đọc/chưa đọc**
            var notificationsByReadStatus = await _context.Notifications
                .GroupBy(n => n.IsRead)
                .Select(g => new
                {
                    IsRead = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var labelsPieChart = new List<string> { "Đã đọc", "Chưa đọc" };
            var dataPieChart = new List<int>
            {
                notificationsByReadStatus.FirstOrDefault(n => n.IsRead)?.Count ?? 0,
                notificationsByReadStatus.FirstOrDefault(n => !n.IsRead)?.Count ?? 0
            };

            // Truyền dữ liệu vào ViewBag
            ViewBag.PendingBrandsCount = pendingBrandsCount;
            ViewBag.PendingBusRoutesCount = pendingBusRoutesCount;
            ViewBag.PendingCoachesCount = pendingCoachesCount;
            ViewBag.Notifications = notifications;

            // Dữ liệu cho biểu đồ
            ViewBag.LabelsAreaChart1 = labelsAreaChart1;
            ViewBag.DataAreaChart1 = dataAreaChart1;
            ViewBag.LabelsAreaChart2 = labelsAreaChart2;
            ViewBag.DataAreaChart2 = dataAreaChart2;
            ViewBag.LabelsBarChart = labelsBarChart;
            ViewBag.DataBarChart = dataBarChart;
            ViewBag.LabelsPieChart = labelsPieChart;
            ViewBag.DataPieChart = dataPieChart;

            return View();
        }
    }
}