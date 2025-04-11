using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class BusRoute
    {
        [Key]
        public int IdRoute { get; set; }
        public string? RouteCode { get; set; }
        public string? NameRoute { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Distance must be greater than 0")]
        public int? Distance { get; set; }
        public BusRouteState State { get; set; }
        [ForeignKey("RegistForm")]
        public int? IdRegist { get; set; }

        public RegistForm? RegistForm { get; set; }
    }
    public enum BusRouteState
    {
        [Display(Name = "Hoạt động")]
        HoatDong = 0,

        [Display(Name = "Không hoạt động")]
        KhongHoatDong = 1
    }
}
