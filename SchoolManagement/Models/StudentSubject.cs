namespace SchoolManagement.Models
{
    public class StudentSubject
    {
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
        
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
    }
} 