namespace SchoolManagement.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string StudentCode { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        
        public int ClassId { get; set; }
        public virtual Class Class { get; set; }
        
        public virtual ICollection<StudentSubject> StudentSubjects { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
    }
} 