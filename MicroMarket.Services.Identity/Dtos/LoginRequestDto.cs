using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Services.Identity.Dtos
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress(ErrorMessage = "The email must be in a valid format (e.g., example@mail.com).")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "The password is required.")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "The password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = string.Empty;
    }
}
