using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TicketBus.Data;
using TicketBus.Models;
using Microsoft.Extensions.Logging;

[Area("Passenger")]
[Authorize(Roles = "Passenger")]
public class TripController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<TripController> _logger;

    public TripController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<TripController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult Index(string startPoint, string destination, DateTime? departureDate, string sortOption,
        string priceRange, string departureTime, int? operatorId, int? vehicleTypeId, int page = 1)
    {
        var cities = _context.Cities
            .Select(c => new { c.IdCity, c.NameCity })
            .OrderBy(c => c.NameCity)
            .ToList();

        var operators = _context.Brands
            .Select(b => new { b.IdBrand, b.NameBrand })
            .OrderBy(b => b.NameBrand)
            .ToList();

        var vehicleTypes = _context.VehicleTypes
            .Select(v => new { v.IdType, v.NameType })
            .OrderBy(v => v.NameType)
            .ToList();
        if (DateTime.TryParseExact(Request.Query["departureDate"], "dd-MM-yy hh:mm:ss tt",
              CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            departureDate = parsedDate;
        }
        ViewBag.Cities = cities;
        ViewBag.Operators = operators;
        ViewBag.VehicleTypes = vehicleTypes;
        ViewBag.StartPoint = startPoint;
        ViewBag.Destination = destination;
        ViewBag.DepartureDate = departureDate?.ToString("yyyy-MM-dd"); // Format cho input date
        ViewBag.SortOption = sortOption;
        ViewBag.PriceRange = priceRange;
        ViewBag.DepartureTime = departureTime;
        ViewBag.OperatorId = operatorId;
        ViewBag.VehicleTypeId = vehicleTypeId;
        ViewBag.PageNumber = page;

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
            .AsSplitQuery()
            .AsQueryable();

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

        if (!string.IsNullOrEmpty(priceRange))
        {
            var range = priceRange.Split('-');
            if (range.Length == 2 && decimal.TryParse(range[0], out var minPrice) && decimal.TryParse(range[1], out var maxPrice))
            {
                pricesQuery = pricesQuery.Where(p => p.PriceValue >= minPrice && p.PriceValue <= maxPrice);
            }
        }

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

        if (operatorId.HasValue)
        {
            pricesQuery = pricesQuery.Where(p => p.ScheduleDetails != null &&
                p.ScheduleDetails.Coach != null &&
                p.ScheduleDetails.Coach.Brand != null &&
                p.ScheduleDetails.Coach.IdBrand == operatorId.Value);
        }

        if (vehicleTypeId.HasValue)
        {
            pricesQuery = pricesQuery.Where(p => p.ScheduleDetails != null &&
                p.ScheduleDetails.Coach != null &&
                p.ScheduleDetails.Coach.VehicleType != null &&
                p.ScheduleDetails.Coach.IdType == vehicleTypeId.Value);
        }

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

    public async Task<IActionResult> Details(int id, DateTime? departureDate)
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

        if (price == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var passenger = _context.Passengers
                .FirstOrDefault(p => p.UserId == user.Id);
            ViewBag.CustomerName = passenger?.NamePassenger ?? user.FullName;
            ViewBag.CustomerPhone = passenger?.PhoneNumber ?? user.PhoneNumber;
        }
        ViewBag.DepartureDate = departureDate;

        var SeatList = _context.Seats
            .Include(s => s.Coach)
            .Where(s => s.Coach.IdCoach == price.ScheduleDetails.IdCoach)
            .ToList();
        ViewBag.SeatList = SeatList;

        // Determine if the vehicle is a sleeper bus
        bool isSleeper = price.ScheduleDetails?.Coach?.VehicleType?.NameType.ToLower().Contains("giường nằm") == true;
        ViewBag.IsSleeper = isSleeper;

        // If it's a sleeper bus, split seats into upper and lower decks
        if (isSleeper)
        {
            // Assuming SeatNumber or another property indicates deck (e.g., SeatNumber < half for upper deck)
            var totalSeats = SeatList.Count;
            var halfSeats = totalSeats / 2;
            ViewBag.UpperDeckSeats = SeatList.Take(halfSeats).ToList();
            ViewBag.LowerDeckSeats = SeatList.Skip(halfSeats).ToList();
        }

        var PickUpList = _context.Pickups
            .Include(p => p.City)
            .Where(p => p.IdRoute == price.ScheduleDetails.BusRoute.IdRoute)
            .ToList();
        ViewBag.PickUpList = PickUpList;

        var DropOffList = _context.DropOffs
            .Include(d => d.City)
            .Where(d => d.IdRoute == price.ScheduleDetails.BusRoute.IdRoute)
            .ToList();
        ViewBag.DropOffList = DropOffList;
        ViewBag.idPrice = price.IdPrice;

        var emptySeatsCount = SeatList.Count(s => s.State == SeatState.Trong);
        ViewBag.EmptySeats = new Dictionary<int, int>
        {
            { price.ScheduleDetails.IdCoach, emptySeatsCount }
        };

        return View(price);
    }

    [HttpPost]
    public async Task<IActionResult> Details(List<int> seatIds, int diemDi, int diemDen, int idPrice, DateTime? departureDate)
    {
        _logger.LogInformation("[Ticket Debug] POST Details: seatIds={seatIds}, diemDi={diemDi}, diemDen={diemDen}, idPrice={idPrice}, departureDate={departureDate}", seatIds, diemDi, diemDen, idPrice, departureDate);
        // Kiểm tra seatIds
        if (seatIds == null || seatIds.Count == 0)
        {
            _logger.LogWarning("[Ticket Debug] Không có seatIds được chọn");
            ModelState.AddModelError(string.Empty, "Vui lòng chọn ít nhất một ghế!");
            return View(await GetPriceDetails(idPrice));
        }
        if (seatIds.Count > 4)
        {
            _logger.LogWarning("[Ticket Debug] Chọn quá số lượng ghế cho phép: {Count}", seatIds.Count);
            ModelState.AddModelError(string.Empty, "Bạn chỉ được chọn tối đa 4 ghế!");
            return View(await GetPriceDetails(idPrice));
        }
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogError("[Ticket Debug] Không tìm thấy thông tin người dùng");
            ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.");
            return View(await GetPriceDetails(idPrice));
        }
        var passenger = await _context.Passengers.FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (passenger == null)
        {
            _logger.LogError("[Ticket Debug] Không tìm thấy thông tin hành khách cho userId={UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin hành khách. Vui lòng liên hệ quản trị viên.");
            return View(await GetPriceDetails(idPrice));
        }
        var price = await _context.Prices
            .Include(p => p.RouteStopStart)
            .Include(p => p.RouteStopEnd)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.Seats)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.Brand)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.VehicleType)
            .FirstOrDefaultAsync(p => p.IdPrice == idPrice);
        if (price == null)
        {
            _logger.LogError("[Ticket Debug] Không tìm thấy thông tin giá với idPrice={IdPrice}", idPrice);
            return NotFound();
        }
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var seatId in seatIds)
            {
                var seat = await _context.Seats.FirstOrDefaultAsync(s => s.IdSeat == seatId && s.State == SeatState.Trong);
                if (seat == null)
                {
                    _logger.LogWarning("[Ticket Debug] Ghế với ID {SeatId} không khả dụng hoặc đã được đặt", seatId);
                    ModelState.AddModelError(string.Empty, $"Ghế với ID {seatId} không khả dụng hoặc đã được đặt.");
                    return View(await GetPriceDetails(idPrice));
                }
                var ticket = new Ticket
                {
                    IdPrice = idPrice,
                    IdSeat = seatId,
                    CreatedDate = DateTime.Now,
                    State = TicketState.ChuaThanhToan,
                    DepartureDate = departureDate
                };
                _logger.LogInformation("[Ticket Debug] Tạo ticket: {@Ticket}", ticket);
                seat.State = SeatState.DaDat;
                _context.Update(seat);
                _context.Tickets.Add(ticket);
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("[Ticket Debug] Lưu vé thành công cho các seatIds: {seatIds}", seatIds);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "[Ticket Debug] Lỗi khi lưu vé");
            ModelState.AddModelError(string.Empty, $"Lỗi khi lưu vé: {ex.Message}");
            return View(await GetPriceDetails(idPrice));
        }
        return RedirectToAction("Payment", new { idPrice = idPrice, seatIds = seatIds, pickupId = diemDi, dropOffId = diemDen });
    }

    // Phương thức hỗ trợ để lấy chi tiết giá
    private async Task<Price> GetPriceDetails(int idPrice)
    {
        return await _context.Prices
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
            .FirstOrDefaultAsync(p => p.IdPrice == idPrice);
    }

    public async Task<IActionResult> Payment(int idPrice, List<int> seatIds, int pickupId, int dropOffId, DateTime? departureDate)
    {
        var price = _context.Prices
            .Include(p => p.RouteStopStart)
            .Include(p => p.RouteStopEnd)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.BusRoute)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.Seats)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.Brand)
            .Include(p => p.ScheduleDetails)
                .ThenInclude(sd => sd.Coach)
                    .ThenInclude(c => c.VehicleType)
            .FirstOrDefault(p => p.IdPrice == idPrice);

        if (!departureDate.HasValue && !string.IsNullOrEmpty(Request.Query["departureDate"]))
        {
            if (DateTime.TryParseExact(Request.Query["departureDate"], "dd-MM-yy hh:mm:ss tt",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                departureDate = parsedDate;
            }
        }

        if (price == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var passenger = await _context.Passengers
            .FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (passenger == null)
        {
            return NotFound();
        }

        var seats = _context.Seats
            .Where(s => seatIds.Contains(s.IdSeat))
            .Select(s => s.SeatCode)
            .ToList();

        var pickup = _context.Pickups
            .Include(p => p.City)
            .FirstOrDefault(p => p.IdPickup == pickupId);

        var dropOff = _context.DropOffs
            .Include(d => d.City)
            .FirstOrDefault(d => d.IdDropOff == dropOffId);

        ViewBag.CustomerName = passenger?.NamePassenger ?? user.FullName;
        ViewBag.CustomerPhone = passenger?.PhoneNumber ?? user.PhoneNumber;
        ViewBag.SelectedSeats = seats;
        ViewBag.Pickup = pickup;
        ViewBag.DropOff = dropOff;
        ViewBag.DepartureDate = departureDate;


        return View(price);
    }
}