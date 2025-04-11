using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class Bill
    {
        [Key]
        public int IdBill { get; set; }
        public string? BillCode { get; set; }
        [ForeignKey("Passenger")]
        public int? IdPassenger { get; set; }
        public int? SeatQuantity { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Total must be greater than or equal to 0")]
        public decimal Total { get; set; }
        public Passenger? Passenger { get; set; }
    }
}
