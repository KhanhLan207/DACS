using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketBus.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } // "VNPay", "MoMo", etc.

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [StringLength(255)]
        public string TransactionCode { get; set; } // Mã giao dịch gửi đi

        [StringLength(255)]
        public string TransactionId { get; set; } // Mã giao dịch từ cổng thanh toán trả về

        [StringLength(500)]
        public string PaymentInfo { get; set; } // Thông tin bổ sung
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Refunded = 3,
        Cancelled = 4
    }
} 