using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketBus.Models
{
    public class Seat
    {
        [Key]
        public int IdSeat { get; set; }
        
        public int IdCoach { get; set; }

        [ForeignKey("IdCoach")]
        public Coach Coach { get; set; }
        
        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; }
        
        [Required]
        [StringLength(10)]
        public string SeatCode { get; set; }
        
        [Required]
        public SeatType Type { get; set; }
        
        public int Row { get; set; }
        
        public int Column { get; set; }
        
        public SeatStatus Status { get; set; }
        
        public SeatState State { get; set; }
    }
    
    public enum SeatType
    {
        Standard = 0,
        Business = 1,
        VIP = 2
    }
    
    public enum SeatStatus
    {
        Available = 0,
        Booked = 1,
        Unavailable = 2
    }
    
    public enum SeatState
    {
        Trong = 0,
        DaDat = 1,
        KhongHoatDong = 2
    }
}
