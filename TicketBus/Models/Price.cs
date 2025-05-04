using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class Price
    {
        [Key]
        public int IdPrice { get; set; }

        public int IdSchedule { get; set; }
        public string? PriceCode { get; set; }
        public decimal PriceValue { get; set; }
        [ForeignKey("BusRoute")]
        public int? IdRoute { get; set; }
        [ForeignKey("RouteStopStart")]
        public int? IdStopStart { get; set; }
        [ForeignKey("RouteStopEnd")]
        public int? IdStopEnd { get; set; }
        [ForeignKey("Coach")]
        public int? IdCoach { get; set; }

        public BusRoute? BusRoute { get; set; }
        public RouteStop? RouteStopStart { get; set; }
        public RouteStop? RouteStopEnd { get; set; }
        public Coach? Coach { get; set; }

        [ForeignKey("IdSchedule")]
        public ScheduleDetails? ScheduleDetails { get; set; }
    }
}
