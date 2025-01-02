using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models.DTO
{
    public class RegistrationDTO
    {
        [Required]
        public string userName { get; set; }
        [Required]
        public string fullName { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Minimum length 6 and must contain  1 Uppercase,1 lowercase, 1 special character and 1 digit")]
        public string password { get; set; }
        [Required]
        [Compare("password")]
        public string passwordConfirm { get; set; }

        [Required]
        public DateTime dateBirthday { get; set; }
        public string? Role { get; set; }
    }
}