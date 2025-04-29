using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace TicketBus.Models
{
    public class RouteStop
    {
        [Key]
        public int IdStop { get; set; }

        public string? StopCode { get; set; }

        [ForeignKey("BusRoute")]
        public int? IdRoute { get; set; }

        public string? StopName { get; set; }

        public string? Address { get; set; }

        [ForeignKey("City")]
        public int? IdCity { get; set; }

        [ForeignKey("District")]
        public int? IdDistrict { get; set; }

        public int? StopOrder { get; set; }

        public TimeSpan? Time { get; set; }

        public BusRoute? BusRoute { get; set; }
        public City? City { get; set; } // Liên kết với City
        public District? District { get; set; } // Liên kết với District
    }
}
