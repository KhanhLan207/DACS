using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketBus.Models
{
    // Chú ý: Kiểm tra xem có lớp Order nào khác trong project không
    public class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Payments = new HashSet<Payment>();
            CreatedAt = DateTime.Now;
            Status = OrderStatus.Created;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? PaymentDate { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public string Note { get; set; }

        // Navigation properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }

    public enum OrderStatus
    {
        Created = 0,
        Pending = 1,
        Paid = 2,
        Completed = 3,
        Cancelled = 4,
        Failed = 5
    }
} 