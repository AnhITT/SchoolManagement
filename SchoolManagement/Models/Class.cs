using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Class
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên lớp")]
        [Display(Name = "Tên lớp")]
        public string ClassName { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        public virtual ICollection<Student>? Students { get; set; }

        public Class()
        {
            Students = new HashSet<Student>();
        }
    }
} 