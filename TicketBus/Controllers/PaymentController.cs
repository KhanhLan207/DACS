using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security;
using System.Threading.Tasks;
using TicketBus.Data;
using TicketBus.Models;
using TicketBus.Services.Momo;

namespace TicketBus.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly ApplicationDbContext _context;

        public PaymentController(IMomoService momoService, ApplicationDbContext context)
        {
            _momoService = momoService ?? throw new ArgumentNullException(nameof(momoService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public IActionResult Index() => View();
        public IActionResult PaymentSuccess()
        {
            return View("PaymentSuccess"); // ✅ Đảm bảo gọi đúng tên view
        }

        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                Console.WriteLine("🔄 Đang xử lý phản hồi thanh toán từ MoMo...");
                Console.WriteLine($"📢 Dữ liệu query từ MoMo: {JsonConvert.SerializeObject(Request.Query)}");

                var response = await _momoService.PaymentExecuteAsync(Request.Query);

                // ✅ Log phản hồi từ MoMo sau khi xử lý
                Console.WriteLine($"📢 Phản hồi từ MoMo: {JsonConvert.SerializeObject(response)}");

                if (response == null || response.PaymentStatus == "Error" || string.IsNullOrEmpty(response.BillCode))
                {
                    Console.WriteLine("❌ Lỗi: Thanh toán thất bại hoặc phản hồi không hợp lệ.");
                    return View("PaymentFailed");
                }

                Console.WriteLine($"✅ Thanh toán thành công! BillCode: {response.BillCode}, Trạng thái: {response.PaymentStatus}");
                return View("PaymentSuccess");
            }
            catch (SecurityException ex)
            {
                Console.WriteLine($"❌ Lỗi bảo mật: {ex.Message}");
                return View("PaymentError");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi hệ thống: {ex.Message}");
                return View("PaymentError");
            }
        }
        [HttpPost]
        [Route("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model, [FromForm] string SelectedSeats)
        {
            // ✅ Log kiểm tra dữ liệu đầu vào
            Console.WriteLine($"🔍 Dữ liệu nhận từ form: {JsonConvert.SerializeObject(model)}");
            Console.WriteLine($"📢 Ghế đã chọn (chuỗi CSV): {SelectedSeats}");

            if (model == null || string.IsNullOrEmpty(SelectedSeats))
            {
                Console.WriteLine("❌ Lỗi: Thông tin đơn hàng hoặc ghế không hợp lệ.");
                return BadRequest(new { message = "Thông tin đơn hàng hoặc danh sách ghế không hợp lệ." });
            }

            // ✅ Chuyển danh sách ghế từ chuỗi CSV thành danh sách
            model.SelectedSeat = SelectedSeats.Split(',').ToList();
            model.SeatQuantity = model.SelectedSeat.Count;

            Console.WriteLine($"📢 Số lượng ghế đã chọn: {model.SeatQuantity}");

            // ✅ Lấy `UserId` từ bảng `AspNetUsers`
            var userEmail = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                Console.WriteLine("❌ Không tìm thấy thông tin user đăng nhập.");
                return BadRequest(new { message = "Bạn cần đăng nhập để thanh toán." });
            }

            // ✅ Tìm `IdPassenger` từ bảng `Passengers`
            var passenger = await _context.Passengers.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (passenger == null || passenger.IdPassenger == 0)
            {
                Console.WriteLine("❌ Không tìm thấy `IdPassenger` trong hệ thống.");
                return BadRequest(new { message = "Không thể xác định hành khách." });
            }

            model.IdPassenger = passenger.IdPassenger;
            Console.WriteLine($"📢 Đã lấy `IdPassenger` từ database: {model.IdPassenger}");

            // ✅ Log kiểm tra dữ liệu trước khi gửi yêu cầu đến MoMo
            Console.WriteLine($"📢 Dữ liệu gửi đi cho MoMo: {JsonConvert.SerializeObject(model)}");

            var response = await _momoService.CreatePaymentAsync(model);

            // ✅ Log phản hồi từ MoMo
            Console.WriteLine($"📢 Phản hồi từ MoMo: {JsonConvert.SerializeObject(response)}");

            if (response == null || string.IsNullOrEmpty(response.PayUrl))
            {
                Console.WriteLine("❌ Lỗi khi tạo liên kết thanh toán.");
                return BadRequest(new { message = "Lỗi xử lý thanh toán." });
            }

            Console.WriteLine($"✅ Đã tạo liên kết thanh toán thành công: {response.PayUrl}");
            return Redirect(response.PayUrl);
        }
    }
}
