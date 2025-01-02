using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã sinh viên")]
        [Display(Name = "Mã sinh viên")]
        public string StudentCode { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        [Display(Name = "Lớp")]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class? Class { get; set; }
        
        public virtual ICollection<StudentSubject>? StudentSubjects { get; set; }
        public virtual ICollection<Attendance>? Attendances { get; set; }

        public Student()
        {
            StudentSubjects = new HashSet<StudentSubject>();
            Attendances = new HashSet<Attendance>();
        }
    }
} 