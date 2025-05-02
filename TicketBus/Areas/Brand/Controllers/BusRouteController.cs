using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketBus.Data;
using TicketBus.Models;
using TicketBus.Models.ViewModels;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace TicketBus.Areas.Brand.Controllers
{
    [Area("Brand")]
    [Authorize(Roles = "Brand")]
    public class BusRouteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BusRouteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Hiển thị form đăng ký tuyến (không điền sẵn dữ liệu)
        public IActionResult Create()
        {
            var model = new BusRouteViewModel
            {
                DepartureTimes = new List<string>(),
                RouteStops = new List<RouteStopViewModel>
                {
                    new RouteStopViewModel(),
                    new RouteStopViewModel()
                },
                Brands = _context.Brands
                    .Select(b => new SelectListItem
                    {
                        Value = b.IdBrand.ToString(),
                        Text = b.NameBrand
                    })
                    .ToList(),
                Cities = _context.Cities
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCity.ToString(),
                        Text = c.NameCity
                    })
                    .ToList()
            };

            foreach (var stop in model.RouteStops)
            {
                stop.Cities = _context.Cities
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCity.ToString(),
                        Text = c.NameCity
                    })
                    .ToList();
                stop.Cities.Insert(0, new SelectListItem { Value = "", Text = "Chọn thành phố" });
            }

            return View(model);
        }

        // POST: Thêm giờ xuất bến
        [HttpPost]
        public IActionResult AddDepartureTime(BusRouteViewModel model)
        {
            if (model.DepartureTimes == null)
            {
                model.DepartureTimes = new List<string>();
            }
            model.DepartureTimes.Add("");

            model.Brands = _context.Brands
                .Select(b => new SelectListItem
                {
                    Value = b.IdBrand.ToString(),
                    Text = b.NameBrand
                })
                .ToList();
            model.Cities = _context.Cities
                .Select(c => new SelectListItem
                {
                    Value = c.IdCity.ToString(),
                    Text = c.NameCity
                })
                .ToList();

            foreach (var stop in model.RouteStops)
            {
                stop.Cities = _context.Cities
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCity.ToString(),
                        Text = c.NameCity
                    })
                    .ToList();
                stop.Cities.Insert(0, new SelectListItem { Value = "", Text = "Chọn thành phố" });
            }

            return View("Create", model);
        }

        // POST: Thêm điểm dừng
        [HttpPost]
        public IActionResult AddRouteStop(BusRouteViewModel model)
        {
            if (model.RouteStops == null)
            {
                model.RouteStops = new List<RouteStopViewModel>();
            }

            foreach (var stop in model.RouteStops)
            {
                stop.Cities = _context.Cities
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCity.ToString(),
                        Text = c.NameCity
                    })
                    .ToList();
                stop.Cities.Insert(0, new SelectListItem { Value = "", Text = "Chưa xác định" });
            }

            var newStop = new RouteStopViewModel
            {
                StopOrder = model.RouteStops.Count,
                Cities = _context.Cities
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCity.ToString(),
                        Text = c.NameCity
                    })
                    .ToList()
            };
            newStop.Cities.Insert(0, new SelectListItem { Value = "", Text = "Chọn thành phố" });
            model.RouteStops.Add(newStop);

            model.Brands = _context.Brands
                .Select(b => new SelectListItem
                {
                    Value = b.IdBrand.ToString(),
                    Text = b.NameBrand
                })
                .ToList();
            model.Cities = _context.Cities
                .Select(c => new SelectListItem
                {
                    Value = c.IdCity.ToString(),
                    Text = c.NameCity
                })
                .ToList();

            return View("Create", model);
        }

        // POST: Xử lý đăng ký tuyến
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BusRouteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var stopNames = model.RouteStops.Select(rs => rs.StopName).ToList();
                if (stopNames.Any(sn => !string.IsNullOrEmpty(sn)) && stopNames.Where(sn => !string.IsNullOrEmpty(sn)).Distinct().Count() != stopNames.Count(sn => !string.IsNullOrEmpty(sn)))
                {
                    ModelState.AddModelError("", "Các điểm dừng (nếu có tên) không được trùng tên.");
                }

                if (model.RouteStops.Count < 2)
                {
                    ModelState.AddModelError("", "Tuyến xe phải có ít nhất 2 điểm dừng (điểm bắt đầu và điểm kết thúc).");
                }

                for (int i = 1; i < model.RouteStops.Count; i++)
                {
                    if (string.IsNullOrEmpty(model.RouteStops[i].Time) || string.IsNullOrEmpty(model.RouteStops[i - 1].Time))
                        continue;

                    var prevTime = TimeSpan.Parse(model.RouteStops[i - 1].Time);
                    var currTime = TimeSpan.Parse(model.RouteStops[i].Time);
                    if (currTime <= prevTime)
                    {
                        ModelState.AddModelError("", "Thời gian của điểm dừng sau phải lớn hơn điểm dừng trước.");
                        break;
                    }
                }

                if (!model.IdStartCity.HasValue || !await _context.Cities.AnyAsync(c => c.IdCity == model.IdStartCity.Value))
                {
                    ModelState.AddModelError("IdStartCity", "Thành phố xuất phát không tồn tại.");
                }
                if (!model.IdEndCity.HasValue || !await _context.Cities.AnyAsync(c => c.IdCity == model.IdEndCity.Value))
                {
                    ModelState.AddModelError("IdEndCity", "Thành phố kết thúc không tồn tại.");
                }

                foreach (var stop in model.RouteStops)
                {
                    if (stop.IdCity.HasValue && !await _context.Cities.AnyAsync(c => c.IdCity == stop.IdCity.Value))
                    {
                        ModelState.AddModelError("", $"Thành phố không hợp lệ cho điểm dừng {stop.StopName ?? "không tên"}.");
                        break;
                    }
                }

                if (model.DepartureTimes == null || !model.DepartureTimes.Any())
                {
                    ModelState.AddModelError("DepartureTimes", "Phải có ít nhất một giờ xuất bến.");
                }
                else
                {
                    foreach (var time in model.DepartureTimes)
                    {
                        if (string.IsNullOrEmpty(time))
                        {
                            ModelState.AddModelError("DepartureTimes", "Giờ xuất bến không được để trống.");
                            break;
                        }

                        if (!Regex.IsMatch(time, @"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$"))
                        {
                            ModelState.AddModelError("DepartureTimes", "Giờ xuất bến phải có định dạng HH:mm (ví dụ: 08:00).");
                            break;
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    model.Brands = _context.Brands
                        .Select(b => new SelectListItem
                        {
                            Value = b.IdBrand.ToString(),
                            Text = b.NameBrand
                        })
                        .ToList();
                    model.Cities = _context.Cities
                        .Select(c => new SelectListItem
                        {
                            Value = c.IdCity.ToString(),
                            Text = c.NameCity
                        })
                        .ToList();

                    foreach (var stop in model.RouteStops)
                    {
                        stop.Cities = _context.Cities
                            .Select(c => new SelectListItem
                            {
                                Value = c.IdCity.ToString(),
                                Text = c.NameCity
                            })
                            .ToList();
                        stop.Cities.Insert(0, new SelectListItem { Value = "", Text = "Chọn thành phố" });
                    }

                    return View(model);
                }

                var brand = await _context.Brands
                    .Include(b => b.RegistForm)
                    .FirstOrDefaultAsync(b => b.IdBrand == model.IdBrand);
                if (brand == null)
                {
                    ModelState.AddModelError("IdBrand", "Hãng xe không tồn tại.");
                    model.Brands = _context.Brands
                        .Select(b => new SelectListItem
                        {
                            Value = b.IdBrand.ToString(),
                            Text = b.NameBrand
                        })
                        .ToList();
                    model.Cities = _context.Cities
                        .Select(c => new SelectListItem
                        {
                            Value = c.IdCity.ToString(),
                            Text = c.NameCity
                        })
                        .ToList();

                    foreach (var stop in model.RouteStops)
                    {
                        stop.Cities = _context.Cities
                            .Select(c => new SelectListItem
                            {
                                Value = c.IdCity.ToString(),
                                Text = c.NameCity
                            })
                            .ToList();
                        stop.Cities.Insert(0, new SelectListItem { Value = "", Text = "Chọn thành phố" });
                    }

                    return View(model);
                }
                int? idRegist = brand.RegistFormId;

                int routeCount = await _context.BusRoutes.CountAsync() + 1;
                string routeCode = $"BR-{routeCount:D3}";

                int stopCount = await _context.RouteStops.CountAsync();
                var routeStops = model.RouteStops.Select((rs, index) =>
                {
                    stopCount++;
                    TimeSpan? timeSpan = null;
                    if (!string.IsNullOrEmpty(rs.Time))
                    {
                        var parts = rs.Time.Split(':');
                        timeSpan = new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);
                    }

                    return new RouteStop
                    {
                        StopCode = $"STOP-{stopCount:D3}",
                        StopName = rs.StopName,
                        IdCity = rs.IdCity,
                        StopOrder = index,
                        Time = timeSpan
                    };
                }).ToList();

                TimeSpan? travelTime = null;
                if (!string.IsNullOrEmpty(model.TravelTime))
                {
                    var parts = model.TravelTime.Split(':');
                    travelTime = new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);
                }

                var departureTimes = model.DepartureTimes.Select(time =>
                {
                    var parts = time.Split(':');
                    return new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);
                }).ToList();

                var busRoute = new BusRoute
                {
                    RouteCode = routeCode,
                    NameRoute = model.NameRoute,
                    Address = model.Address,
                    Distance = model.Distance,
                    IdBrand = model.IdBrand,
                    IdRegist = idRegist,
                    IdStartCity = model.IdStartCity,
                    IdEndCity = model.IdEndCity,
                    State = BusRouteState.ChoPheDuyet,
                    TravelTime = travelTime,
                    DepartureTimes = departureTimes,
                    StartDate = model.StartDate,
                    RouteStops = routeStops
                };

                _context.Add(busRoute);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký tuyến xe thành công! Vui lòng chờ admin phê duyệt.";
                return RedirectToAction(nameof(Index));
            }

            model.Brands = _context.Brands
                .Select(b => new SelectListItem
                {
                    Value = b.IdBrand.ToString(),
                    Text = b.NameBrand
                })
                .ToList();
            model.Cities = _context.Cities
                .Select(c => new SelectListItem
                {
                    Value = c.IdCity.ToString(),
                    Text = c.NameCity
                })
                .ToList();

            foreach (var stop in model.RouteStops)
            {
                stop.Cities = _context.Cities
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCity.ToString(),
                        Text = c.NameCity
                    })
                    .ToList();
                stop.Cities.Insert(0, new SelectListItem { Value = "", Text = "Chọn thành phố" });
            }

            return View(model);
        }

        // GET: Hiển thị danh sách tuyến xe của Brand hiện tại, đã được phê duyệt
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
                .FirstOrDefaultAsync(b => b.UserId == userId);

            if (brand == null)
            {
                TempData["Message"] = "Hãng xe của bạn chưa được phê duyệt hoặc không tồn tại.";
                return View(new List<BusRoute>());
            }

            var busRoutes = await _context.BusRoutes
                .Where(r => r.IdBrand == brand.IdBrand && r.State == BusRouteState.DaPheDuyet)
                .Include(r => r.Brand)
                .Include(r => r.StartCity)
                .Include(r => r.EndCity)
                .Include(r => r.RouteStops)
                .ToListAsync();

            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            return View(busRoutes);
        }

        // GET: /Brand/BusRoute/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Không thể xác định người dùng. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var brand = await _context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.UserId == userId);
            if (brand == null)
            {
                TempData["Message"] = "Hãng xe của bạn chưa được phê duyệt hoặc không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            var busRoute = await _context.BusRoutes
                .Include(r => r.StartCity)
                .Include(r => r.EndCity)
                .Include(r => r.Brand)
                .Include(r => r.RouteStops)
                .FirstOrDefaultAsync(r => r.IdRoute == id && r.IdBrand == brand.IdBrand);

            if (busRoute == null)
            {
                return NotFound();
            }

            return View(busRoute);
        }
    }
}