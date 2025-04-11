using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class Brand
    {
        [Key]
        public int IdBrand { get; set; }
        public string? BrandCode { get; set; }
        public string? NameBrand { get; set; }
        public string? Address { get; set; }
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "PhoneNumber must contain only digits")]
        public string? PhoneNumber { get; set; }
        public BrandState State { get; set; }
    }
    public enum BrandState
    {
        [Display(Name = "Hoạt động")]
        HoatDong = 0,

        [Display(Name = "Không hoạt động")]
        KhongHoatDong = 1
    }
}
