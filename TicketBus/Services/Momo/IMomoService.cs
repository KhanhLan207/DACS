using System.Threading.Tasks;

namespace TicketBus.Services.Momo
{
    public interface IMomoService
    {
        Task<string> CreatePaymentAsync(long amount, string orderInfo, string orderId);
        Task<MomoResponse> ProcessPaymentResponseAsync(string queryString);
    }

    public class MomoResponse
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public long Amount { get; set; }
        public string Message { get; set; }
        public string ResponseCode { get; set; }
    }
} 