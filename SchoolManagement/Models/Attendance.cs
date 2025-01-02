using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        public int SessionId { get; set; }
        public virtual Session Session { get; set; }

        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        [Required]
        [Display(Name = "Có mặt")]
        public bool IsPresent { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Display(Name = "Thời gian điểm danh")]
        public DateTime? AttendanceTime { get; set; }
    }
} 