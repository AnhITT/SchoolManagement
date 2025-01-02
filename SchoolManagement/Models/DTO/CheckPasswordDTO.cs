using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models.DTO
{
    public class CheckPasswordDTO
    {
        [Key]
        [Required]
        public string id { get; set; }

        public string? name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string newPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string repeatPassword { get; set; }
    }
}