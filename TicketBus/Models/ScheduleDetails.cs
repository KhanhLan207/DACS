using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class ScheduleDetails
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Coach")]
        public int IdCoach { get; set; }
        [Key, Column(Order = 1)]
        [ForeignKey("BusRoute")]
        public int IdRoute { get; set; }
        public TimeSpan? DepartTime { get; set; }
        public TimeSpan? ArriveTime { get; set; }

        public Coach? Coach { get; set; }
        public BusRoute? BusRoute { get; set; }
    }
}
