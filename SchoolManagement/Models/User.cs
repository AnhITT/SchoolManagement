using Microsoft.AspNetCore.Identity;

namespace SchoolManagement.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
    }
}