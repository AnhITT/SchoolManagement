namespace SchoolManagement.Models
{
    public class Session
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
        
        public DateTime SessionDate { get; set; }
        public string Room { get; set; }
        public string Description { get; set; }
        
        public virtual ICollection<Attendance> Attendances { get; set; }
    }
} 