using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.ViewModels
{
    public class AddStudentToSubjectViewModel
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public List<StudentInSubjectViewModel> Students { get; set; }
    }

    public class StudentInSubjectViewModel
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; }
        public string FullName { get; set; }
        public string ClassName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsRegistered { get; set; }
    }
} 