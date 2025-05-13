using Microsoft.AspNetCore.Mvc;
using TicketBus.Models.Order;
using TicketBus.Services.Momo;

namespace TicketBus.Areas.Passenger.Controllers
{
    public class PaymentController : Controller
    {

        private IMomoService _momoService;
        public PaymentController(IMomoService momoService)
        {
            _momoService = momoService;

        }
        [HttpPost]
        [Route("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model)
        {
            var response = await _momoService.CreatePaymentAsync(model);
            return Redirect(response.PayUrl);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
