using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using TicketBus.Models;

namespace TicketBus.Services
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(long amount, string orderInfo, string orderType, string orderCode);
        VNPayResponse ProcessResponse(IQueryCollection collections);
    }

    public class VNPayService : IVNPayService
    {
        private readonly VNPaySettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VNPayService(IOptions<VNPaySettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            _settings = settings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public string CreatePaymentUrl(long amount, string orderInfo, string orderType, string orderCode)
        {
            var pay = new VNPayLibrary();
            var request = _httpContextAccessor.HttpContext.Request;
            
            pay.AddRequestData("vnp_Version", _settings.Version);
            pay.AddRequestData("vnp_Command", _settings.Command);
            pay.AddRequestData("vnp_TmnCode", _settings.TmnCode);
            pay.AddRequestData("vnp_Amount", (amount * 100).ToString()); // VNPay amount is in VND * 100
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _settings.CurrCode);
            pay.AddRequestData("vnp_IpAddr", GetIpAddress());
            pay.AddRequestData("vnp_Locale", _settings.Locale);
            pay.AddRequestData("vnp_OrderInfo", orderInfo);
            pay.AddRequestData("vnp_OrderType", orderType);
            pay.AddRequestData("vnp_ReturnUrl", _settings.ReturnUrl);
            pay.AddRequestData("vnp_TxnRef", orderCode); // This should be a unique reference for the transaction

            var paymentUrl = pay.CreateRequestUrl(_settings.PaymentUrl, _settings.HashSecret);
            return paymentUrl;
        }

        public VNPayResponse ProcessResponse(IQueryCollection collections)
        {
            var vnpay = new VNPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }
            
            var orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnPayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnpResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnpSecureHash = collections.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value;
            var checkSignature = vnpay.ValidateSignature(vnpSecureHash, _settings.HashSecret);

            var response = new VNPayResponse
            {
                Success = checkSignature && vnpResponseCode == "00",
                PaymentMethod = "VNPay",
                OrderId = orderId.ToString(),
                TransactionId = vnPayTranId.ToString(),
                Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100,
                Token = vnpSecureHash,
                PaymentDate = DateTime.ParseExact(vnpay.GetResponseData("vnp_PayDate"), "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                ResponseCode = vnpResponseCode
            };

            return response;
        }

        private string GetIpAddress()
        {
            string ipAddress;
            try
            {
                var request = _httpContextAccessor.HttpContext.Request;
                ipAddress = request.Headers["X-Forwarded-For"].FirstOrDefault();

                if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
                    ipAddress = request.Headers["REMOTE_ADDR"].ToString();

                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();

                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = "127.0.0.1";

                return ipAddress;
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }

    public class VNPayResponse
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public long Amount { get; set; }
        public string Token { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ResponseCode { get; set; }
    }
} 