using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class Coach
    {
        [Key]
        public int IdCoach { get; set; }
        [StringLength(50)]
        public string? CoachCode { get; set; }
        [StringLength(10)]
        public string? NumberPlate { get; set; }
        public CoachState State { get; set; }
        [ForeignKey("VehicleType")]
        public int? IdType { get; set; }
        [ForeignKey("RegistForm")]
        public int? IdRegist { get; set; }
        public List<string> Images { get; set; } = new List<string>();

        public List<string> Documents { get; set; } = new List<string>();

        public VehicleType? VehicleType { get; set; }
        public RegistForm? RegistForm { get; set; }

        [ForeignKey("Brand")]
        public int? IdBrand { get; set; }

        public Brand? Brand { get; set; }
    }

    public enum CoachState
    {
        [Display(Name = "Hoạt động")]
        HoatDong = 0,

        [Display(Name = "Không hoạt động")]
        KhongHoatDong =1,

        [Display(Name = "Chờ phê duyệt")]
        ChoPheDuyet =2
    }
}
