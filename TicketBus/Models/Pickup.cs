using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class Pickup
    {
        [Key]
        public int IdPickup { get; set; }
        public string? PickupCode { get; set; }
        public string? PickupName { get; set; }
        [ForeignKey("City")]
        public int? IdCity { get; set; }
        [ForeignKey("RegistForm")]
        public int? IdRegist { get; set; }
        public string? Address { get; set; }

        public City? City { get; set; }
        public RegistForm? RegistForm { get; set; }
    }
}
