namespace SchoolManagement.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        
        public virtual ICollection<StudentSubject> StudentSubjects { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
} 