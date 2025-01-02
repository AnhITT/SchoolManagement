using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.ViewModels
{
    public class AttendanceViewModel
    {
        public int StudentId { get; set; }
        
        [Display(Name = "Có mặt")]
        public bool IsPresent { get; set; }
        
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }
} 