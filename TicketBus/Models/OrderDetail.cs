using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketBus.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int TripId { get; set; }
        
        [ForeignKey("TripId")]
        public Trip Trip { get; set; }

        public int? SeatId { get; set; }

        [ForeignKey("SeatId")]
        public Seat Seat { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        [StringLength(100)]
        public string PassengerName { get; set; }

        [StringLength(20)]
        public string PassengerPhone { get; set; }

        public string Note { get; set; }
    }
} 