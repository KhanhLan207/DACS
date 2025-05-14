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
            try
            {
                // Add debug information
                System.Diagnostics.Debug.WriteLine($"Checkout initiated for orderId: {orderId}");
                
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Bạn cần đăng nhập để tiếp tục.";
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout", new { orderId }) });
                }

                // Lấy thông tin đơn hàng từ database
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Trip)
                    .ThenInclude(t => t.Route)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Order not found: {orderId}");
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng.";
                    return RedirectToAction("Index", "Home");
                }
                
                // Log order details for debugging
                System.Diagnostics.Debug.WriteLine($"Order found: {order.Id}, Status: {order.Status}, User: {order.UserId}");
                
                // Check if order belongs to current user
                if (order.UserId != user.Id)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập đơn hàng này.";
                    return RedirectToAction("Index", "Home");
                }

                // Check if order is in the correct state for payment
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Created)
                {
                    if (order.Status == OrderStatus.Paid || order.Status == OrderStatus.Completed)
                    {
                        TempData["SuccessMessage"] = "Đơn hàng này đã được thanh toán.";
                        return RedirectToAction("PaymentSuccess", new { orderId = order.Id });
                    }
                    
                    TempData["ErrorMessage"] = "Đơn hàng này không thể thanh toán.";
                    return RedirectToAction("Index", "Home");
                }

                // For clearer debugging, log the order details we're about to send to the view
                System.Diagnostics.Debug.WriteLine($"Creating checkout model for order: {order.Id}");
                System.Diagnostics.Debug.WriteLine($"Order has {order.OrderDetails?.Count ?? 0} details");

                // Fix for possible null references
                if (order.OrderDetails == null || !order.OrderDetails.Any())
                {
                    System.Diagnostics.Debug.WriteLine("Order has no details!");
                    TempData["ErrorMessage"] = "Đơn hàng không có chi tiết. Vui lòng thử đặt vé lại.";
                    return RedirectToAction("Index", "Home");
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
                        RouteName = od.Trip?.Route?.NameRoute ?? "Unknown Route",
                        DepartureDate = od.Trip?.DepartureDate ?? DateTime.Now,
                        DepartureTime = od.Trip?.DepartureTime.ToString(@"hh\:mm") ?? "00:00",
                        Price = od.Price
                    }).ToList()
                };

                // Log that we're about to return the view
                System.Diagnostics.Debug.WriteLine("Returning checkout view with model");
                return View(model);
            }
            catch (Exception ex)
            {
                // Comprehensive error logging
                System.Diagnostics.Debug.WriteLine($"Error in Checkout/Index: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Log inner exception if any
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
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