using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class Bill
    {
        [Key]
        public int IdBill { get; set; }

        public string? BillCode { get; set; }

        public int? SeatQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total must be greater than or equal to 0")]
        public decimal Total { get; set; }

        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
        public decimal DiscountPercentage { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discounted amount must be greater than or equal to 0")]
        public decimal DiscountedAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Final total must be greater than or equal to 0")]
        public decimal FinalTotal { get; set; }

        [ForeignKey("Passenger")]
        public int? IdPassenger { get; set; }

        public Passenger? Passenger { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
