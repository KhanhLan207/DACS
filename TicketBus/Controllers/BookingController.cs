using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketBus.Data;
using TicketBus.Models;

namespace TicketBus.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public BookingController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Bước 1: Tìm chuyến
        public async Task<IActionResult> Search(string from, string to, DateTime? date)
        {
            // Mặc định là ngày hiện tại nếu không chọn
            date ??= DateTime.Today;
            
            ViewBag.Cities = await _context.Cities.ToListAsync();
            ViewBag.SelectedFrom = from;
            ViewBag.SelectedTo = to;
            ViewBag.SelectedDate = date.Value;
            
            // Tìm các tuyến đường phù hợp
            var routes = await _context.BusRoutes
                .Include(r => r.StartCity)
                .Include(r => r.EndCity)
                .Include(r => r.Brand)
                .Where(r => 
                    (string.IsNullOrEmpty(from) || r.StartCity.NameCity.Contains(from)) &&
                    (string.IsNullOrEmpty(to) || r.EndCity.NameCity.Contains(to)) &&
                    r.State == BusRouteState.DaPheDuyet)
                .ToListAsync();

            // Tìm các chuyến đi trong ngày được chọn
            var trips = await _context.Trips
                .Include(t => t.Route)
                    .ThenInclude(r => r.StartCity)
                .Include(t => t.Route)
                    .ThenInclude(r => r.EndCity)
                .Include(t => t.Coach)
                    .ThenInclude(c => c.Brand)
                .Include(t => t.Coach)
                    .ThenInclude(c => c.VehicleType)
                .Where(t => 
                    t.DepartureDate.Date == date.Value.Date &&
                    (string.IsNullOrEmpty(from) || t.Route.StartCity.NameCity.Contains(from)) &&
                    (string.IsNullOrEmpty(to) || t.Route.EndCity.NameCity.Contains(to)))
                .ToListAsync();

            // Tính số ghế trống cho mỗi chuyến
            var tripInfo = new Dictionary<int, int>();
            foreach (var trip in trips)
            {
                var bookedSeats = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .Where(o => o.Status != OrderStatus.Cancelled)
                    .SelectMany(o => o.OrderDetails)
                    .Where(od => od.TripId == trip.Id)
                    .CountAsync();
                
                var totalSeats = await _context.Seats
                    .Where(s => s.IdCoach == trip.CoachId && s.State != SeatState.KhongHoatDong)
                    .CountAsync();
                
                tripInfo[trip.Id] = totalSeats - bookedSeats;
            }
            
            ViewBag.AvailableSeats = tripInfo;
            
            return View(trips);
        }
        
        // Bước 2: Chọn ghế
        public async Task<IActionResult> SelectSeats(int tripId)
        {
            var trip = await _context.Trips
                .Include(t => t.Route)
                    .ThenInclude(r => r.StartCity)
                .Include(t => t.Route)
                    .ThenInclude(r => r.EndCity)
                .Include(t => t.Coach)
                    .ThenInclude(c => c.Brand)
                .Include(t => t.Coach)
                    .ThenInclude(c => c.VehicleType)
                .FirstOrDefaultAsync(t => t.Id == tripId);
            
            if (trip == null)
            {
                return NotFound();
            }
            
            // Lấy danh sách ghế của xe
            var seats = await _context.Seats
                .Where(s => s.IdCoach == trip.CoachId)
                .OrderBy(s => s.SeatNumber)
                .ToListAsync();
            
            // Xác định ghế đã đặt
            var bookedSeatIds = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SelectMany(o => o.OrderDetails)
                .Where(od => od.TripId == tripId)
                .Select(od => od.SeatId.Value)
                .ToListAsync();
            
            ViewBag.BookedSeatIds = bookedSeatIds;
            ViewBag.Trip = trip;
            
            // Lấy giá vé cho các điểm đầu-cuối mặc định
            var price = await _context.Prices
                .Where(p => p.IdSchedule == trip.RouteId) // Giả định là dùng RouteId thay cho IdSchedule
                .OrderByDescending(p => p.PriceValue)
                .FirstOrDefaultAsync();
            
            ViewBag.BasePrice = price?.PriceValue ?? 0;
            
            return View(seats);
        }
        
        // Bước 3: Thông tin hành khách
        [HttpPost]
        public async Task<IActionResult> PassengerInfo(int tripId, List<int> selectedSeats)
        {
            if (selectedSeats == null || !selectedSeats.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một ghế.";
                return RedirectToAction(nameof(SelectSeats), new { tripId });
            }
            
            var trip = await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Coach)
                    .ThenInclude(c => c.Brand)
                .FirstOrDefaultAsync(t => t.Id == tripId);
            
            if (trip == null)
            {
                return NotFound();
            }
            
            // Lấy thông tin ghế đã chọn
            var seats = await _context.Seats
                .Where(s => selectedSeats.Contains(s.IdSeat))
                .ToListAsync();
            
            // Lấy điểm đón trả
            var pickups = await _context.Pickups
                .Where(p => p.IdBrand == trip.Coach.Brand.IdBrand)
                .Include(p => p.City)
                .ToListAsync();
            
            var dropoffs = await _context.DropOffs
                .Where(d => d.IdBrand == trip.Coach.Brand.IdBrand)
                .Include(d => d.City)
                .ToListAsync();
            
            // Tính tổng tiền
            var price = await _context.Prices
                .Where(p => p.IdSchedule == trip.RouteId) // Giả định là dùng RouteId thay cho IdSchedule
                .OrderByDescending(p => p.PriceValue)
                .FirstOrDefaultAsync();
            
            decimal totalAmount = price?.PriceValue * seats.Count ?? 0;
            
            ViewBag.Trip = trip;
            ViewBag.Seats = seats;
            ViewBag.Pickups = pickups;
            ViewBag.Dropoffs = dropoffs;
            ViewBag.TotalAmount = totalAmount;
            
            // Nếu đã đăng nhập, lấy thông tin người dùng
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                ViewBag.UserInfo = user;
            }
            
            return View();
        }
        
        // Bước 4: Xác nhận và thanh toán
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Confirm(BookingInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Thông tin không hợp lệ. Vui lòng kiểm tra lại.";
                return RedirectToAction(nameof(PassengerInfo), new { tripId = model.TripId, selectedSeats = model.SelectedSeatIds });
            }
            
            var trip = await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Coach)
                    .ThenInclude(c => c.Brand)
                .FirstOrDefaultAsync(t => t.Id == model.TripId);
            
            if (trip == null)
            {
                return NotFound();
            }
            
            // Kiểm tra ghế còn trống không
            var bookedSeatIds = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SelectMany(o => o.OrderDetails)
                .Where(od => od.TripId == model.TripId)
                .Select(od => od.SeatId.Value)
                .ToListAsync();
            
            // Kiểm tra xem có ghế nào đã được đặt rồi không
            var conflictSeats = model.SelectedSeatIds.Intersect(bookedSeatIds).ToList();
            if (conflictSeats.Any())
            {
                TempData["ErrorMessage"] = "Có ghế đã được đặt bởi người khác. Vui lòng chọn lại.";
                return RedirectToAction(nameof(SelectSeats), new { tripId = model.TripId });
            }
            
            // Lấy user hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Confirm", "Booking") });
            }
            
            // Tính tổng tiền
            var price = await _context.Prices
                .Where(p => p.IdSchedule == trip.RouteId) // Giả định là dùng RouteId thay cho IdSchedule
                .OrderByDescending(p => p.PriceValue)
                .FirstOrDefaultAsync();
            
            decimal totalAmount = price?.PriceValue * model.SelectedSeatIds.Count ?? 0;
            
            // Tạo đơn hàng mới
            var order = new Order
            {
                UserId = user.Id,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                Note = model.Note ?? ""
            };
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            // Tạo chi tiết đơn hàng cho từng ghế
            for (int i = 0; i < model.SelectedSeatIds.Count; i++)
            {
                var seatId = model.SelectedSeatIds[i];
                var passengerName = model.PassengerNames[i];
                var passengerPhone = model.PassengerPhones[i];
                
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    TripId = trip.Id,
                    SeatId = seatId,
                    Price = price?.PriceValue ?? 0,
                    PassengerName = passengerName,
                    PassengerPhone = passengerPhone,
                    Note = model.Note ?? ""
                };
                
                _context.Add(orderDetail);
            }
            
            await _context.SaveChangesAsync();
            
            // Chuyển đến trang thanh toán
            return RedirectToAction("Index", "Checkout", new { orderId = order.Id });
        }
    }
    
    public class BookingInfoViewModel
    {
        public int TripId { get; set; }
        public List<int> SelectedSeatIds { get; set; }
        public List<string> PassengerNames { get; set; }
        public List<string> PassengerPhones { get; set; }
        public int PickupId { get; set; }
        public int DropOffId { get; set; }
        public string Note { get; set; }
    }
} 