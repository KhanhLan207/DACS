using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string? Image { get; set; }
        public BrandState State { get; set; }

        public int? RegistFormId { get; set; }
        public RegistForm? RegistForm { get; set; }

        // Thêm trường UserId để liên kết với ApplicationUser
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
    public enum BrandState
    {
        [Display(Name = "Hoạt động")]
        HoatDong = 0,

        [Display(Name = "Không hoạt động")]
        KhongHoatDong = 1,

        [Display(Name = "Chờ phê duyệt")]
        ChoPheDuyet = 2
    }
}
