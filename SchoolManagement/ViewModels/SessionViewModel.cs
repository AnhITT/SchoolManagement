using SchoolManagement.Models;

public class SessionViewModel
{
    public Session Session { get; set; }
    public bool IsUpcoming { get; set; }
    public bool IsPast { get; set; }
    public bool IsToday { get; set; }
    public int TotalStudents { get; set; }
    public int PresentCount { get; set; }
} 