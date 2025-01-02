using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã môn học")]
        [Display(Name = "Mã môn học")]
        public string SubjectCode { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên môn học")]
        [Display(Name = "Tên môn học")]
        public string SubjectName { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        
        public virtual ICollection<StudentSubject>? StudentSubjects { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }

        public Subject()
        {
            StudentSubjects = new HashSet<StudentSubject>();
            Sessions = new HashSet<Session>();
        }
    }
} 