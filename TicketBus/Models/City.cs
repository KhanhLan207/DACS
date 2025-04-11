using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class City
    {
        [Key]
        public int IdCity { get; set; }
        public string? CityCode { get; set; }
        public string? NameCity { get; set; }
    }
}
