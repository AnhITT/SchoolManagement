using Microsoft.AspNetCore.Identity;

namespace SchoolManagement.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime DateBirthdayUser { get; set; }
        public string Role { get; set; }
    }
}