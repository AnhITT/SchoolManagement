using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Session
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên buổi học")]
        [Display(Name = "Tên buổi học")]
        public string SessionName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày học")]
        [Display(Name = "Ngày học")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ bắt đầu")]
        [Display(Name = "Giờ bắt đầu")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ kết thúc")]
        [Display(Name = "Giờ kết thúc")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Ghi chú")]
        public string Description { get; set; }

        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }

        public virtual ICollection<Attendance> Attendances { get; set; }

        public Session()
        {
            Attendances = new HashSet<Attendance>();
        }
    }
} 