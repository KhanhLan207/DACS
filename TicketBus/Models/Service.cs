using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class Service
    {
        [Key]
        public int IdService { get; set; }
        public string? ServiceCode { get; set; }
        public string? NameService { get; set; }
    }
}
