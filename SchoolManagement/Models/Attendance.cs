namespace SchoolManagement.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
        
        public int SessionId { get; set; }
        public virtual Session Session { get; set; }
        
        public bool IsPresent { get; set; }
        public string Note { get; set; }
        public DateTime CheckTime { get; set; }
    }
} 