using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using TicketBus.Data;
using TicketBus.Models;
using Microsoft.AspNetCore.Identity;

[Area("Passenger")]
[Authorize(Roles = "Passenger")]
public class TripController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TripController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index(string startPoint, string destination, DateTime? departureDate, string sortOption,
        string priceRange, string departureTime, int? operatorId, int? vehicleTypeId, int page = 1)
    {
        // Lấy danh sách tỉnh và huyện để hiển thị trong dropdown
        var cities = _context.Cities
            .Select(c => new { c.IdCity, c.NameCity })
            .OrderBy(c => c.NameCity)
            .ToList();

        // Lấy danh sách nhà xe và loại xe cho bộ lọc
        var operators = _context.Brands
            .Select(b => new { b.IdBrand, b.NameBrand })
            .OrderBy(b => b.NameBrand)
            .ToList();

        var vehicleTypes = _context.VehicleTypes
            .Select(v => new { v.IdType, v.NameType })
            .OrderBy(v => v.NameType)
            .ToList();

        // Truyền danh sách qua ViewBag
        ViewBag.Cities = cities;
        ViewBag.Operators = operators;
        ViewBag.VehicleTypes = vehicleTypes;
        ViewBag.StartPoint = startPoint;
        ViewBag.Destination = destination;
        ViewBag.DepartureDate = departureDate?.ToString("yyyy-MM-dd");
        ViewBag.SortOption = sortOption;
        ViewBag.PriceRange = priceRange;
        ViewBag.DepartureTime = departureTime;
        ViewBag.OperatorId = operatorId;
        ViewBag.VehicleTypeId = vehicleTypeId;
        ViewBag.PageNumber = page;

        // Truy vấn giá vé và thông tin chuyến xe
        var pricesQuery = _context.Prices
            .Include(p => p.RouteStopStart)
            .Include(p => p.RouteStopEnd)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
                    .ThenInclude(r => r.RouteStops)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.Seats)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.Brand)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.VehicleType)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
                    .ThenInclude(r => r.Pickups)
                        .ThenInclude(p => p.City)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
                    .ThenInclude(r => r.DropOffs)
                        .ThenInclude(d => d.City)
            .AsSplitQuery() // Cải thiện hiệu suất
            .AsQueryable();

        // Lọc theo điểm bắt đầu và điểm đến
        if (!string.IsNullOrEmpty(startPoint) && !string.IsNullOrEmpty(destination))
        {
            startPoint = startPoint.ToLower();
            destination = destination.ToLower();

            pricesQuery = pricesQuery.Where(p =>
                p.ScheduleDetails != null &&
                p.ScheduleDetails.BusRoute != null &&
                p.RouteStopStart != null &&
                p.RouteStopEnd != null &&
                p.RouteStopStart.StopName.ToLower().Contains(startPoint) &&
                p.RouteStopEnd.StopName.ToLower().Contains(destination));
        }

        // Lọc theo khoảng giá
        if (!string.IsNullOrEmpty(priceRange))
        {
            var range = priceRange.Split('-');
            if (range.Length == 2 && decimal.TryParse(range[0], out var minPrice) && decimal.TryParse(range[1], out var maxPrice))
            {
                pricesQuery = pricesQuery.Where(p => p.PriceValue >= minPrice && p.PriceValue <= maxPrice);
            }
        }

        // Lọc theo thời gian khởi hành
        if (!string.IsNullOrEmpty(departureTime))
        {
            switch (departureTime)
            {
                case "morning":
                    pricesQuery = pricesQuery.Where(p => p.ScheduleDetails != null &&
                        p.ScheduleDetails.DepartTime != null &&
                        p.ScheduleDetails.DepartTime.Hours >= 6 &&
                        p.ScheduleDetails.DepartTime.Hours < 12);
                    break;
                case "afternoon":
                    pricesQuery = pricesQuery.Where(p => p.ScheduleDetails != null &&
                        p.ScheduleDetails.DepartTime != null &&
                        p.ScheduleDetails.DepartTime.Hours >= 12 &&
                        p.ScheduleDetails.DepartTime.Hours < 18);
                    break;
                case "evening":
                    pricesQuery = pricesQuery.Where(p => p.ScheduleDetails != null &&
                        p.ScheduleDetails.DepartTime != null &&
                        p.ScheduleDetails.DepartTime.Hours >= 18);
                    break;
            }
        }

        // Lọc theo nhà xe
        if (operatorId.HasValue)
        {
            pricesQuery = pricesQuery.Where(p => p.ScheduleDetails != null &&
                p.ScheduleDetails.Coach != null &&
                p.ScheduleDetails.Coach.Brand != null &&
                p.ScheduleDetails.Coach.IdBrand == operatorId.Value);
        }

        // Lọc theo loại xe
        if (vehicleTypeId.HasValue)
        {
            pricesQuery = pricesQuery.Where(p => p.ScheduleDetails != null &&
                p.ScheduleDetails.Coach != null &&
                p.ScheduleDetails.Coach.VehicleType != null &&
                p.ScheduleDetails.Coach.IdType == vehicleTypeId.Value);
        }

        // Sắp xếp
        switch (sortOption)
        {
            case "earliest":
                pricesQuery = pricesQuery
                    .Where(p => p.RouteStopStart != null && p.RouteStopStart.Time != null)
                    .OrderBy(p => p.RouteStopStart.Time);
                break;

            case "price_asc":
                pricesQuery = pricesQuery.OrderBy(p => p.PriceValue);
                break;
            case "price_desc":
                pricesQuery = pricesQuery.OrderByDescending(p => p.PriceValue);
                break;
            default:
                pricesQuery = pricesQuery.OrderBy(p => p.IdPrice);
                break;
        }

        var prices = pricesQuery.ToList();
        // Tính số ghế trống lại
        var emptySeats = new Dictionary<int, int>();
        foreach (var price in prices)
        {
            if (price?.ScheduleDetails?.Coach?.Seats != null)
            {
                emptySeats[price.ScheduleDetails.IdCoach] = price.ScheduleDetails.Coach.Seats
                    .Count(s => s.State == SeatState.Trong);
            }
            else
            {
                emptySeats[price?.ScheduleDetails?.IdCoach ?? 0] = 0;
            }
        }
        ViewBag.EmptySeats = emptySeats;

        return View(prices);
    }

    public async Task<IActionResult> Details(int id)
    {
        var price = _context.Prices
            .Include(p => p.RouteStopStart)
            .Include(p => p.RouteStopEnd)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
                    .ThenInclude(r => r.RouteStops)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.Seats)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.Brand)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.VehicleType)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
                    .ThenInclude(r => r.Pickups)
                        .ThenInclude(p => p.City)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
                    .ThenInclude(r => r.DropOffs)
                        .ThenInclude(d => d.City)
            .FirstOrDefault(p => p.IdPrice == id);

        // Get current user
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var passenger = _context.Passengers
                .FirstOrDefault(p => p.UserId == user.Id);
            ViewBag.CustomerName = passenger?.NamePassenger ?? user.FullName;
            ViewBag.CustomerPhone = passenger?.PhoneNumber ?? user.PhoneNumber;
        }

        // Danh sách ghế
        var SeatList = _context.Seats
            .Include(s => s.Coach)
            .Where(s => s.Coach.IdCoach == price.ScheduleDetails.IdCoach)
            .ToList();
        ViewBag.SeatList = SeatList;

        // Danh sách điểm đón
        var PickUpList = _context.Pickups
            .Include(p => p.City)
            .Where(p => p.IdRoute == price.ScheduleDetails.BusRoute.IdRoute)
            .ToList();
        ViewBag.PickUpList = PickUpList;

        // Danh sách điểm trả
        var DropOffList = _context.DropOffs
            .Include(d => d.City)
            .Where(d => d.IdRoute == price.ScheduleDetails.BusRoute.IdRoute)
            .ToList();
        ViewBag.DropOffList = DropOffList;
        ViewBag.idPrice = price.IdPrice;

        if (price == null)
        {
            return NotFound();
        }

        var m = _context.Prices
            .Include(p => p.RouteStopStart)
            .Include(p => p.RouteStopEnd)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
                    .ThenInclude(r => r.RouteStops)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.Seats)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.Brand)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.VehicleType)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
                    .ThenInclude(r => r.Pickups)
                        .ThenInclude(p => p.City)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
                    .ThenInclude(r => r.DropOffs)
                        .ThenInclude(d => d.City)
            .AsSplitQuery()
            .AsQueryable().ToList();

        // Tính số ghế trống lại
        var emptySeats = new Dictionary<int, int>();
        foreach (var i in m)
        {
            if (price?.ScheduleDetails?.Coach?.Seats != null)
            {
                emptySeats[price.ScheduleDetails.IdCoach] = i.ScheduleDetails.Coach.Seats
                    .Count(s => s.State == SeatState.Trong);
            }
            else
            {
                emptySeats[price?.ScheduleDetails?.IdCoach ?? 0] = 0;
            }
        }
        ViewBag.EmptySeats = emptySeats;

        return View(price);
    }

    [HttpPost]
    public async Task<IActionResult> Details(List<int> SoGhe, int diemDi, int diemDen, int idPrice)
    {
        if (SoGhe == null || SoGhe.Count <= 0)
        {
            var price = _context.Prices
             .Include(p => p.RouteStopStart)
             .Include(p => p.RouteStopEnd)
             .Include(p => p.ScheduleDetails)
                 .ThenInclude(sd => sd.BusRoute)
                     .ThenInclude(r => r.RouteStops)
             .Include(p => p.ScheduleDetails)
                 .ThenInclude(sd => sd.Coach)
                     .ThenInclude(c => c.Seats)
             .Include(p => p.ScheduleDetails)
                 .ThenInclude(sd => sd.Coach)
                     .ThenInclude(c => c.Brand)
             .Include(p => p.ScheduleDetails)
                 .ThenInclude(sd => sd.Coach)
                     .ThenInclude(c => c.VehicleType)
             .Include(p => p.ScheduleDetails)
                 .ThenInclude(sd => sd.BusRoute)
                     .ThenInclude(r => r.Pickups)
                         .ThenInclude(p => p.City)
             .Include(p => p.ScheduleDetails)
                 .ThenInclude(sd => sd.BusRoute)
                     .ThenInclude(r => r.DropOffs)
                         .ThenInclude(d => d.City)
             .FirstOrDefault(p => p.IdPrice == idPrice);

            // Danh sách ghế
            var SeatList = _context.Seats
                .Include(s => s.Coach)
                .Where(s => s.Coach.IdCoach == price.ScheduleDetails.IdCoach)
                .ToList();
            ViewBag.SeatList = SeatList;

            // Danh sách điểm đón
            var PickUpList = _context.Pickups
                .Include(p => p.City)
                .Where(p => p.IdRoute == price.ScheduleDetails.BusRoute.IdRoute)
                .ToList();
            ViewBag.PickUpList = PickUpList;

            // Danh sách điểm trả
            var DropOffList = _context.DropOffs
                .Include(d => d.City)
                .Where(d => d.IdRoute == price.ScheduleDetails.BusRoute.IdRoute)
                .ToList();
            ViewBag.DropOffList = DropOffList;
            ViewBag.idPrice = price.IdPrice;
            ModelState.AddModelError(string.Empty, "Chọn ghế!!!");
            return View(price);
        }

        // Get current user and corresponding Passenger
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.");
            return View("Details", new { id = idPrice }); // Redirect back to Details GET
        }

        var passenger = await _context.Passengers
            .FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (passenger == null)
        {
            ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin hành khách. Vui lòng liên hệ quản trị viên.");
            return View("Details", new { id = idPrice }); // Redirect back to Details GET
        }

        foreach (var item in SoGhe)
        {
            Ticket x = new Ticket();
            x.IdPrice = idPrice;
            x.IdSeat = item;
            x.CreatedDate = DateTime.Now;
            x.IdPassenger = passenger.IdPassenger; // Use the logged-in user's Passenger ID
            x.State = TicketState.ChuaThanhToan;
            var seat = _context.Seats.FirstOrDefault(s => s.IdSeat == item);
            if (seat != null)
            {
                seat.State = SeatState.DaDat;
                _context.Update(seat);
            }
            _context.Tickets.Add(x);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}