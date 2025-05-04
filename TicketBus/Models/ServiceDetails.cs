using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class ServiceDetails
    {
        [Key, Column(Order = 0)]
        [ForeignKey("VehicleType")]
        public int IdType { get; set; }


        [Key, Column(Order = 1)]
        [ForeignKey("Service")]
        public int IdService { get; set; }

        public VehicleType? VehicleType { get; set; }
        public Service? Service { get; set; }
    }
}
