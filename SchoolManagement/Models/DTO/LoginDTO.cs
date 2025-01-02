using Microsoft.Build.Framework;

namespace SchoolManagement.Models.DTO
{
    public class LoginDTO
    {
        [Required]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }
    }
}