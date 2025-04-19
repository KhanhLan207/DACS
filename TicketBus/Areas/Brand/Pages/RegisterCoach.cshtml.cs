using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TicketBus.Data;
using TicketBus.Models;


namespace TicketBus.Areas.Brand.Pages
{
    [Authorize(Roles = "Brand")]
    public class RegisterCoachModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterCoachModel> _logger;

        public RegisterCoachModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<RegisterCoachModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

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
            public CoachState State { get; set; } = CoachState.ChoPheDuyet;

            [Display(Name = "Ảnh xe")]
            public IFormFile ImageFile { get; set; }

            [Required(ErrorMessage = "Vui lòng upload tài liệu (PDF hoặc Word).")]
            [Display(Name = "Tài liệu (PDF hoặc Word)")]
            public IFormFile DocumentFile { get; set; }
        }

        public List<VehicleType> VehicleTypes { get; set; }

        public bool HasRegisteredBrand { get; set; }

        public async Task OnGetAsync()
        {
            VehicleTypes = await _context.VehicleTypes
                .Where(vt => vt.State == VehicleTypeState.HoatDong)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                Input = new InputModel
                {
                    PhoneNumber = user.PhoneNumber
                };

                // Kiểm tra xem tài khoản đã đăng ký hãng xe chưa
                HasRegisteredBrand = await _context.Brands.AnyAsync(b => b.UserId == user.Id);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            VehicleTypes = await _context.VehicleTypes
                .Where(vt => vt.State == VehicleTypeState.HoatDong)
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Kiểm tra xem tài khoản đã đăng ký hãng xe chưa
            var existingBrand = await _context.Brands
                .FirstOrDefaultAsync(b => b.UserId == user.Id);

            if (existingBrand != null)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã đăng ký một hãng xe. Mỗi tài khoản chỉ được phép đăng ký một hãng xe.");
                return Page();
            }

            Models.Brand brand = new Models.Brand
            {
                BrandCode = $"BRAND-{DateTime.Now:yyyyMMddHHmmss}",
                NameBrand = Input.BrandName,
                Address = Input.BrandAddress,
                PhoneNumber = Input.PhoneNumber,
                State = BrandState.HoatDong,
                UserId = user.Id
            };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/coaches");
            Directory.CreateDirectory(imagesDir);

            var documentsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents");
            Directory.CreateDirectory(documentsDir);

            // Lưu ảnh
            string imagePath = null;
            if (Input.ImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(Input.ImageFile.FileName);
                var filePath = Path.Combine(imagesDir, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await Input.ImageFile.CopyToAsync(stream);
                imagePath = $"/images/coaches/{fileName}";
            }

            // Lưu tài liệu
            string documentPath = null;
            if (Input.DocumentFile != null)
            {
                var docFileName = Guid.NewGuid() + Path.GetExtension(Input.DocumentFile.FileName);
                var docFilePath = Path.Combine(documentsDir, docFileName);
                using var stream = new FileStream(docFilePath, FileMode.Create);
                await Input.DocumentFile.CopyToAsync(stream);
                documentPath = $"/documents/{docFileName}";
            }

            var registForm = new RegistForm
            {
                RegistCode = $"REGIST-{DateTime.Now:yyyyMMddHHmmss}",
                IdBrand = brand.IdBrand,
                CreateDate = DateTime.Now,
                State = RegistFormState.ChuaXuLy,
                Content = $"Yêu cầu đăng ký xe: Biển số {Input.NumberPlate}, Loại xe ID {Input.IdType}, Tài liệu: {documentPath}"
            };
            _context.RegistForms.Add(registForm);
            await _context.SaveChangesAsync();

            var coach = new Coach
            {
                CoachCode = $"COACH-{DateTime.Now:yyyyMMddHHmmss}",
                NumberPlate = Input.NumberPlate,
                State = CoachState.ChoPheDuyet,
                IdType = Input.IdType,
                IdRegist = registForm.IdRegist,
                IdBrand = brand.IdBrand,
                Image = imagePath,
                Document = documentPath
            };

            var vehicleType = await _context.VehicleTypes
                .FirstOrDefaultAsync(vt => vt.IdType == Input.IdType);

            if (vehicleType == null)
            {
                ModelState.AddModelError(string.Empty, "Loại xe không hợp lệ.");
                return Page();
            }

            try
            {
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

                _logger.LogInformation("Đăng ký Coach thành công: BrandName={BrandName}, CoachCode={CoachCode}", Input.BrandName, coach.CoachCode);

                TempData["Message"] = "Đơn đăng ký của bạn đã được chuyển đến để phê duyệt.";
                return RedirectToAction("Index", "Home", new { area = "Brand" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký coach cho user {Email}", user.Email);
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi đăng ký xe. Vui lòng thử lại.");
                return Page();
            }
        }
    }
}