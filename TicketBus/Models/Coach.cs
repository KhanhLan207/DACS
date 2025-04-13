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
        public string? Image { get; set; }

        public string? Document { get; set; }

        public VehicleType? VehicleType { get; set; }
        public RegistForm? RegistForm { get; set; }
    }

    public enum CoachState
    {
        [Display(Name = "Hoạt động")]
        HoatDong,

        [Display(Name = "Không hoạt động")]
        KhongHoatDong
    }
}
