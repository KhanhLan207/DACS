using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketBus.Models
{
    public class ScheduleDetails
    {
        [Key]
        public int IdSchedule { get; set; }
        [Required(ErrorMessage = "Xe là bắt buộc")]
        [ForeignKey("Coach")]
        public int IdCoach { get; set; }

        [Required(ErrorMessage = "Tuyến xe là bắt buộc")]
        [ForeignKey("BusRoute")]
        public int IdRoute { get; set; }

        [Required(ErrorMessage = "Ngày khởi hành là bắt buộc")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Giờ khởi hành là bắt buộc")]
        public TimeSpan DepartTime { get; set; }

        [Required(ErrorMessage = "Giờ đến là bắt buộc")]
        [CustomValidation(typeof(ScheduleDetails), nameof(ValidateArriveTime))]
        public TimeSpan ArriveTime { get; set; }

        public ScheduleState State { get; set; }

        public Coach Coach { get; set; }
        public BusRoute BusRoute { get; set; }

        // Validation method để kiểm tra ArriveTime
        public static ValidationResult ValidateArriveTime(TimeSpan arriveTime, ValidationContext context)
        {
            var instance = (ScheduleDetails)context.ObjectInstance;
            if (instance.DepartTime != default && arriveTime <= instance.DepartTime)
            {
                return new ValidationResult("Giờ đến phải lớn hơn giờ khởi hành trong cùng một ngày.");
            }
            return ValidationResult.Success;
        }
    }

    public enum ScheduleState
    {
        [Display(Name = "Sắp chạy")]
        SapChay = 0,

        [Display(Name = "Đang chạy")]
        DangChay = 1,

        [Display(Name = "Đã hoàn thành")]
        DaHoanThanh = 2,

        [Display(Name = "Đã hủy")]
        DaHuy = 3
    }
}
