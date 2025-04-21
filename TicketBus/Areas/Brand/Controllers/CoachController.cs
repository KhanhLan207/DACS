using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketBus.Data;
using TicketBus.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TicketBus.Areas.Brand.Controllers
{
    [Area("Brand")]
    [Authorize(Roles = "Brand")]
    public class CoachController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoachController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Brand/Coach/Register
        public async Task<IActionResult> Register()
        {
            // Lấy danh sách loại xe (VehicleType) để hiển thị trong dropdown
            var vehicleTypes = await _context.VehicleTypes
                .Where(vt => vt.State == VehicleTypeState.HoatDong)
                .Select(vt => new SelectListItem
                {
                    Value = vt.IdType.ToString(),
                    Text = vt.NameType + " (" + vt.SeatCount + " ghế)"
                })
                .ToListAsync();

            // Debug: Kiểm tra số lượng loại xe
            Console.WriteLine($"Số lượng loại xe tìm được: {vehicleTypes.Count}");
            foreach (var vt in vehicleTypes)
            {
                Console.WriteLine($"Loại xe: {vt.Text}, Value: {vt.Value}");
            }

            ViewBag.VehicleTypes = vehicleTypes;

            // Lấy IdBrand của hãng xe hiện tại
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var brand = await _context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.UserId == userId && b.State == BrandState.HoatDong);

            if (brand == null)
            {
                TempData["Message"] = "Hãng xe của bạn chưa được phê duyệt hoặc không tồn tại.";
                return RedirectToAction("Index", "Home");
            }

            // Tạo model Coach mới với IdBrand được điền sẵn
            var coach = new Coach
            {
                IdBrand = brand.IdBrand,
                State = CoachState.ChoPheDuyet // Mặc định trạng thái là "Chờ phê duyệt"
            };

            return View(coach);
        }

        // POST: /Brand/Coach/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Coach coach, IFormFile[] imageFiles, IFormFile[] documentFiles)
        {
            if (ModelState.IsValid)
            {
                // Debug: Kiểm tra số lượng file hình ảnh nhận được
                if (imageFiles != null)
                {
                    Console.WriteLine($"Số lượng file hình ảnh nhận được: {imageFiles.Length}");
                    foreach (var file in imageFiles)
                    {
                        Console.WriteLine($"File hình ảnh: {file.FileName}, Size: {file.Length}");
                    }
                }
                else
                {
                    Console.WriteLine("Không nhận được file hình ảnh nào.");
                }

                // Debug: Kiểm tra số lượng file tài liệu nhận được
                if (documentFiles != null)
                {
                    Console.WriteLine($"Số lượng file tài liệu nhận được: {documentFiles.Length}");
                    foreach (var file in documentFiles)
                    {
                        Console.WriteLine($"File tài liệu: {file.FileName}, Size: {file.Length}");
                    }
                }
                else
                {
                    Console.WriteLine("Không nhận được file tài liệu nào.");
                }

                // Xử lý upload nhiều hình ảnh
                if (imageFiles != null && imageFiles.Length > 0)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/coaches");
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    foreach (var imageFile in imageFiles)
                    {
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                            var imageFullPath = Path.Combine(imagePath, imageFileName);

                            using (var stream = new FileStream(imageFullPath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(stream);
                            }

                            coach.Images.Add("/images/coaches/" + imageFileName);
                        }
                    }
                }

                // Xử lý upload nhiều tài liệu (PDF và Word)
                if (documentFiles != null && documentFiles.Length > 0)
                {
                    var docPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents/coaches");
                    if (!Directory.Exists(docPath))
                    {
                        Directory.CreateDirectory(docPath);
                    }

                    foreach (var documentFile in documentFiles)
                    {
                        if (documentFile != null && documentFile.Length > 0)
                        {
                            // Kiểm tra định dạng file (chỉ cho phép PDF và Word)
                            var extension = Path.GetExtension(documentFile.FileName).ToLower();
                            if (extension == ".pdf" || extension == ".doc" || extension == ".docx")
                            {
                                var docFileName = Guid.NewGuid().ToString() + extension;
                                var docFullPath = Path.Combine(docPath, docFileName);

                                using (var stream = new FileStream(docFullPath, FileMode.Create))
                                {
                                    await documentFile.CopyToAsync(stream);
                                }

                                coach.Documents.Add("/documents/coaches/" + docFileName);
                            }
                            else
                            {
                                ModelState.AddModelError("documentFiles", $"File {documentFile.FileName} không hợp lệ. Chỉ chấp nhận file PDF hoặc Word (.doc, .docx).");
                            }
                        }
                    }
                }

                // Nếu có lỗi validation (ví dụ: file không hợp lệ), trả lại form
                if (!ModelState.IsValid)
                {
                    var vehicleTypes = await _context.VehicleTypes
                        .Where(vt => vt.State == VehicleTypeState.HoatDong)
                        .Select(vt => new SelectListItem
                        {
                            Value = vt.IdType.ToString(),
                            Text = vt.NameType + " (" + vt.SeatCount + " ghế)"
                        })
                        .ToListAsync();

                    ViewBag.VehicleTypes = vehicleTypes;
                    return View(coach);
                }

                // Lưu Coach vào database
                _context.Add(coach);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Đăng ký xe thành công! Xe đang chờ phê duyệt.";
                return RedirectToAction("Index", "Home");
            }

            // Nếu ModelState không hợp lệ, trả lại form với danh sách loại xe
            var vehicleTypesError = await _context.VehicleTypes
                .Where(vt => vt.State == VehicleTypeState.HoatDong)
                .Select(vt => new SelectListItem
                {
                    Value = vt.IdType.ToString(),
                    Text = vt.NameType + " (" + vt.SeatCount + " ghế)"
                })
                .ToListAsync();

            ViewBag.VehicleTypes = vehicleTypesError;
            return View(coach);
        }
    }
}