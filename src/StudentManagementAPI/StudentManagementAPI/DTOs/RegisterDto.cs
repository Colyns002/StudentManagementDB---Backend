using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.DTOs
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
         ErrorMessage = "Password must have uppercase, lowercase, number, and special character.")]
        public string Password { get; set; } = string.Empty;
    }
}