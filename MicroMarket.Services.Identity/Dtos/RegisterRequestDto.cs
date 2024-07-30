using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Services.Identity.Dtos
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "The name is required.")]
        [RegularExpression("^[А-ЯЁA-Z][а-яёa-z]{1,29}$", ErrorMessage = "The name must start with an uppercase letter, contain only letters, and be between 2 and 30 characters long.")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "The surname is required.")]
        [RegularExpression("^[А-ЯЁA-Z][а-яёa-z]{1,29}$", ErrorMessage = "The surname must start with an uppercase letter, contain only letters, and be between 2 and 30 characters long.")]
        public string Surname { get; set; } = string.Empty;


        [RegularExpression("^[А-ЯЁA-Z][а-яёa-z]{1,29}$", ErrorMessage = "The middle name must start with an uppercase letter, contain only letters, and be between 2 and 30 characters long.")]
        public string MiddleName { get; set; } = string.Empty;


        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress(ErrorMessage = "The email must be in a valid format (e.g., example@mail.com).")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "The password is required.")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "The password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "The confirmation password is required.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}