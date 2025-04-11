using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class DropOff
    {
        [Key]
        public int IdDropOff { get; set; }
        public string? DropOffCode { get; set; }
        public string? DropOffName { get; set; }
        [ForeignKey("City")]
        public int? IdCity { get; set; }
        [ForeignKey("RegistForm")]
        public int? IdRegist { get; set; }
        public string? Address { get; set; }

        public City? City { get; set; }
        public RegistForm? RegistForm { get; set; }
    }
}
