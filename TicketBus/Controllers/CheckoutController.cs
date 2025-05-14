using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TicketBus.Data;
using TicketBus.Models;
using TicketBus.Services;
using TicketBus.Services.Momo;

namespace TicketBus.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMomoService _momoService;
        private readonly IVNPayService _vnPayService;

        public CheckoutController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IMomoService momoService,
            IVNPayService vnPayService)
        {
            _context = context;
            _userManager = userManager;
            _momoService = momoService;
            _vnPayService = vnPayService;
        }

        [Authorize]
        public async Task<IActionResult> Index(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout", new { orderId }) });
            }

            // Lấy thông tin đơn hàng từ database
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Trip)
                .ThenInclude(t => t.Route)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            // Tạo model cho view
            var model = new OrderCheckoutViewModel
            {
                OrderId = order.Id.ToString(),
                CustomerName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                TotalAmount = order.TotalAmount,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailViewModel
                {
                    RouteName = od.Trip.Route.NameRoute,
                    DepartureDate = od.Trip.DepartureDate,
                    DepartureTime = od.Trip.DepartureTime.ToString(@"hh\:mm"),
                    Price = od.Price
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult VnPayCheckout(long amount, string orderInfo, string orderId)
        {
            // Kiểm tra đơn hàng hợp lệ
            var order = _context.Orders.FirstOrDefault(o => o.Id.ToString() == orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng";
                return RedirectToAction("Index", "Home");
            }

            // Generate a unique order code
            var orderCode = DateTime.Now.Ticks.ToString() + "-" + orderId;
            
            // Lưu thông tin thanh toán
            var payment = new Payment
            {
                OrderId = order.Id,
                PaymentMethod = "VNPay",
                Amount = amount,
                PaymentDate = DateTime.Now,
                Status = PaymentStatus.Pending,
                TransactionCode = orderCode
            };
            _context.Payments.Add(payment);
            _context.SaveChanges();
            
            // Create payment URL
            var url = _vnPayService.CreatePaymentUrl(amount, orderInfo, "other", orderCode);
            
            // Redirect to VNPAY
            return Redirect(url);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MomoCheckout(long amount, string orderInfo, string orderId)
        {
            // Kiểm tra đơn hàng hợp lệ
            var order = _context.Orders.FirstOrDefault(o => o.Id.ToString() == orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng";
                return RedirectToAction("Index", "Home");
            }

            // Lưu thông tin thanh toán
            var payment = new Payment
            {
                OrderId = order.Id,
                PaymentMethod = "MoMo",
                Amount = amount,
                PaymentDate = DateTime.Now,
                Status = PaymentStatus.Pending,
                TransactionCode = orderId
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Tạo URL thanh toán
            var payUrl = await _momoService.CreatePaymentAsync(amount, orderInfo, orderId);

            // Chuyển hướng người dùng đến trang thanh toán MoMo
            return Redirect(payUrl);
        }

        [HttpGet]
        public async Task<IActionResult> VnPayReturn()
        {
            var response = _vnPayService.ProcessResponse(Request.Query);
            
            if (response.Success)
            {
                // Extract orderId from orderCode (format: timestamp-orderId)
                var orderCodeParts = response.OrderId.Split('-');
                if (orderCodeParts.Length != 2 || !int.TryParse(orderCodeParts[1], out int orderId))
                {
                    TempData["ErrorMessage"] = "Mã đơn hàng không hợp lệ";
                    return RedirectToAction("PaymentError");
                }
                
                // Update payment status in database
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.TransactionCode == response.OrderId);
                
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.TransactionId = response.TransactionId;
                    payment.PaymentDate = response.PaymentDate;

                    // Update order status
                    var order = await _context.Orders.FindAsync(orderId);
                    if (order != null)
                    {
                        order.Status = OrderStatus.Paid;
                        order.PaymentDate = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();
                }
                
                // Redirect to success page
                TempData["SuccessMessage"] = "Thanh toán thành công!";
                return RedirectToAction("PaymentSuccess", new { orderId = orderCodeParts[1] });
            }
            
            // Payment failed
            TempData["ErrorMessage"] = $"Lỗi thanh toán: {response.ResponseCode}";
            return RedirectToAction("PaymentError");
        }

        [HttpGet]
        public async Task<IActionResult> MomoReturn()
        {
            var paymentResponse = await _momoService.ProcessPaymentResponseAsync(Request.QueryString.Value);
            
            if (paymentResponse.Success)
            {
                // Update payment status in database
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.TransactionCode == paymentResponse.OrderId);
                
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.TransactionId = paymentResponse.TransactionId;

                    // Update order status
                    var order = await _context.Orders.FindAsync(int.Parse(paymentResponse.OrderId));
                    if (order != null)
                    {
                        order.Status = OrderStatus.Paid;
                        order.PaymentDate = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();
                }
                
                // Redirect to success page
                TempData["SuccessMessage"] = "Thanh toán thành công!";
                return RedirectToAction("PaymentSuccess", new { orderId = paymentResponse.OrderId });
            }
            
            // Payment failed
            TempData["ErrorMessage"] = $"Lỗi thanh toán: {paymentResponse.ResponseCode}";
            return RedirectToAction("PaymentError");
        }

        public IActionResult PaymentSuccess(string orderId)
        {
            // Display success page with order information
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.OrderId = orderId;
            return View();
        }

        public IActionResult PaymentError()
        {
            // Display error page
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? "Có lỗi xảy ra trong quá trình thanh toán";
            return View();
        }
    }

    public class OrderCheckoutViewModel
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }

    public class OrderDetailViewModel
    {
        public string RouteName { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public decimal Price { get; set; }
    }
} 