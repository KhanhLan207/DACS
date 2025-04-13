using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TicketBus.Data;
using TicketBus.Models;

namespace TicketBus.Areas.Brand.Controllers
{
    [Area("Brand")]
    [Authorize(Roles = "Brand")]
    public class CoachController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CoachController> _logger;

        public CoachController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<CoachController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập tên hãng xe.")]
            [StringLength(100, ErrorMessage = "Tên hãng xe phải từ {2} đến {1} ký tự.", MinimumLength = 2)]
            [Display(Name = "Tên hãng xe")]
            public string BrandName { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập địa chỉ hãng xe.")]
            [StringLength(200, ErrorMessage = "Địa chỉ hãng xe phải từ {2} đến {1} ký tự.", MinimumLength = 5)]
            [Display(Name = "Địa chỉ hãng xe")]
            public string BrandAddress { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
            [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số điện thoại chỉ được chứa số.")]
            [Display(Name = "Số điện thoại liên hệ")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập biển số xe.")]
            [RegularExpression(@"^[0-9]{2}[A-Z]-[0-9]{3}\.[0-9]{2}$", ErrorMessage = "Biển số xe không hợp lệ. Định dạng: 51B-123.45")]
            [Display(Name = "Biển số xe")]
            public string NumberPlate { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn loại xe.")]
            [Display(Name = "Loại xe")]
            public int IdType { get; set; }

            [Display(Name = "Tình trạng xe")]
            public CoachState State { get; set; } = CoachState.HoatDong;

            [Display(Name = "Ảnh xe")]
            public IFormFile ImageFile { get; set; }

            [Display(Name = "Tài liệu (Word hoặc PDF)")]
            public IFormFile DocumentFile { get; set; }
        }

        // GET: /Brand/Coach/RegisterCoach
        public async Task<IActionResult> RegisterCoach()
        {
            var vehicleTypes = await _context.VehicleTypes
                .Where(vt => vt.State == VehicleTypeState.HoatDong)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            var model = new InputModel
            {
                PhoneNumber = user?.PhoneNumber
            };

            ViewBag.VehicleTypes = vehicleTypes;
            return View(model);
        }

        // POST: /Brand/Coach/UploadTempDocument
        [HttpPost]
        public async Task<IActionResult> UploadTempDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn file." });
            }

            var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, message = "Chỉ chấp nhận file Word (.doc, .docx) hoặc PDF (.pdf)." });
            }

            try
            {
                var documentFileName = Guid.NewGuid().ToString() + extension;
                var documentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/guides", documentFileName);
                using (var stream = new FileStream(documentFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var documentPath = $"/guides/{documentFileName}";
                return Json(new { success = true, documentPath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload temporary document.");
                return Json(new { success = false, message = "Có lỗi xảy ra khi upload file. Vui lòng thử lại." });
            }
        }

        // POST: /Brand/Coach/RegisterCoach
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCoach(InputModel input)
        {
            var vehicleTypes = await _context.VehicleTypes
                .Where(vt => vt.State == VehicleTypeState.HoatDong)
                .ToListAsync();
            ViewBag.VehicleTypes = vehicleTypes;

            // Xử lý upload tài liệu Word/PDF (lưu vào wwwroot/guides)
            string documentPath = null;
            if (input.DocumentFile != null)
            {
                var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
                var extension = Path.GetExtension(input.DocumentFile.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("DocumentFile", "Chỉ chấp nhận file Word (.doc, .docx) hoặc PDF (.pdf).");
                }
                else
                {
                    var documentFileName = Guid.NewGuid().ToString() + extension;
                    var documentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/guides", documentFileName);
                    using (var stream = new FileStream(documentFilePath, FileMode.Create))
                    {
                        await input.DocumentFile.CopyToAsync(stream);
                    }
                    documentPath = $"/guides/{documentFileName}";
                    TempData["DocumentPath"] = documentPath;
                }
            }
            else if (TempData["DocumentPath"] != null)
            {
                documentPath = TempData["DocumentPath"].ToString();
                TempData.Keep("DocumentPath");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                var existingBrand = await _context.Brands
                    .FirstOrDefaultAsync(b => b.UserId == user.Id);
                if (existingBrand != null)
                {
                    ModelState.AddModelError(string.Empty, "Bạn đã đăng ký thông tin hãng xe rồi. Vui lòng chỉnh sửa thông tin thay vì tạo mới.");
                    return View(input);
                }

                // Xử lý upload ảnh (lưu vào wwwroot/images/coaches)
                string imagePath = null;
                if (input.ImageFile != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(input.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/coaches", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await input.ImageFile.CopyToAsync(stream);
                    }
                    imagePath = $"/images/coaches/{fileName}";
                }

                var brand = new TicketBus.Models.Brand
                {
                    BrandCode = $"BRAND-{DateTime.Now:yyyyMMddHHmmss}",
                    NameBrand = input.BrandName,
                    Address = input.BrandAddress,
                    PhoneNumber = input.PhoneNumber,
                    State = BrandState.HoatDong,
                    UserId = user.Id
                };

                var coach = new Coach
                {
                    CoachCode = $"COACH-{DateTime.Now:yyyyMMddHHmmss}",
                    NumberPlate = input.NumberPlate,
                    State = input.State,
                    IdType = input.IdType,
                    Image = imagePath,
                    Document = documentPath
                };

                var vehicleType = await _context.VehicleTypes
                    .FirstOrDefaultAsync(vt => vt.IdType == input.IdType);
                if (vehicleType == null)
                {
                    ModelState.AddModelError(string.Empty, "Loại xe không hợp lệ.");
                    return View(input);
                }

                try
                {
                    _context.Brands.Add(brand);
                    _context.Coaches.Add(coach);
                    await _context.SaveChangesAsync();

                    for (int i = 1; i <= vehicleType.SeatCount; i++)
                    {
                        var seat = new Seat
                        {
                            SeatCode = $"SEAT-{coach.IdCoach}-{i}",
                            SeatNumber = i,
                            State = SeatState.Trong,
                            IdCoach = coach.IdCoach
                        };
                        _context.Seats.Add(seat);
                    }
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Brand and Coach registered successfully: BrandName={BrandName}, CoachCode={CoachCode}", input.BrandName, coach.CoachCode);

                    TempData["Message"] = "Đăng ký xe thành công!";
                    TempData["DocumentPath"] = documentPath;

                    return RedirectToAction("Index", "Home", new { area = "Brand" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to register coach for user {Email}", user.Email);
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi đăng ký xe. Vui lòng thử lại.");
                }
            }

            return View(input);
        }
    }
}
