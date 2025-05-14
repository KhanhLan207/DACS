using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TicketBus.Models.Momo;

namespace TicketBus.Services.Momo
{
    public class MomoService : IMomoService
    {
        private readonly MomoOptionModel _options;
        private readonly HttpClient _httpClient;

        public MomoService(IOptions<MomoOptionModel> options, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> CreatePaymentAsync(long amount, string orderInfo, string orderId)
        {
            string requestId = Guid.NewGuid().ToString();
            string orderIdAndRequestId = $"{orderId}_{requestId}";
            string rawHash = $"partnerCode={_options.PartnerCode}" +
                             $"&accessKey={_options.AccessKey}" +
                             $"&requestId={requestId}" +
                             $"&amount={amount}" +
                             $"&orderId={orderIdAndRequestId}" +
                             $"&orderInfo={orderInfo}" +
                             $"&returnUrl={_options.ReturnUrl}" +
                             $"&notifyUrl={_options.NotifyUrl}" +
                             $"&extraData=";

            string signature = ComputeHmacSha256(rawHash, _options.SecretKey);

            var requestData = new
            {
                partnerCode = _options.PartnerCode,
                accessKey = _options.AccessKey,
                requestId = requestId,
                amount = amount,
                orderId = orderIdAndRequestId,
                orderInfo = orderInfo,
                returnUrl = _options.ReturnUrl,
                notifyUrl = _options.NotifyUrl,
                extraData = "",
                requestType = _options.RequestType,
                signature = signature
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_options.MomoApiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

            if (responseData.TryGetValue("payUrl", out string payUrl))
            {
                return payUrl;
            }

            throw new Exception("Không thể tạo liên kết thanh toán Momo");
        }

        public async Task<MomoResponse> ProcessPaymentResponseAsync(string queryString)
        {
            var data = JObject.Parse(queryString);
            
            string signatureFromMomo = data["signature"]?.ToString();
            
            // Construct signature data to verify
            string rawHash = $"partnerCode={data["partnerCode"]}" +
                             $"&accessKey={_options.AccessKey}" +
                             $"&requestId={data["requestId"]}" +
                             $"&amount={data["amount"]}" +
                             $"&orderId={data["orderId"]}" +
                             $"&orderInfo={data["orderInfo"]}" +
                             $"&orderType={data["orderType"]}" +
                             $"&transId={data["transId"]}" +
                             $"&message={data["message"]}" +
                             $"&responseTime={data["responseTime"]}" +
                             $"&resultCode={data["resultCode"]}" +
                             $"&extraData={data["extraData"]}";

            string computedSignature = ComputeHmacSha256(rawHash, _options.SecretKey);

            // Verify signature
            bool isValidSignature = signatureFromMomo == computedSignature;
            bool isSuccessful = data["resultCode"]?.ToString() == "0";

            // Get original orderId
            string fullOrderId = data["orderId"]?.ToString() ?? "";
            string orderId = fullOrderId.Split('_')[0]; // Extract the original order ID

            return new MomoResponse
            {
                Success = isValidSignature && isSuccessful,
                OrderId = orderId,
                TransactionId = data["transId"]?.ToString(),
                Amount = long.Parse(data["amount"]?.ToString() ?? "0"),
                PaymentMethod = "MoMo",
                Message = data["message"]?.ToString(),
                ResponseCode = data["resultCode"]?.ToString()
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                StringBuilder builder = new StringBuilder();
                
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                
                return builder.ToString();
            }
        }
    }
} 