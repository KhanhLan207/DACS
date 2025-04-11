using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class Ticket
    {
        [Key]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "IdTicket must be alphanumeric")]
        public int IdTicket { get; set; }
        public string? TicketCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        [ForeignKey("Seat")]
        public int? IdSeat { get; set; }
        [ForeignKey("Price")]
        public int? IdPrice { get; set; }
        [ForeignKey("Passenger")]
        public int? IdPassenger { get; set; }
        public TicketState State { get; set; }
        [ForeignKey("Employee")]
        public int? IdEmployee { get; set; }

        public Seat? Seat { get; set; }
        public Price? Price { get; set; }
        public Passenger? Passenger { get; set; }
        public Employee? Employee { get; set; }
    }

    public enum TicketState
    {
        [Display(Name = "Chưa thanh toán")]
        ChuaThanhToan = 0,

        [Display(Name = "Đã thanh toán")]
        DaThanhToan = 1
    }
}
