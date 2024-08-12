namespace MicroMarket.Services.Ordering.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DeliveryAddress
    {
        public Guid Id { get; set; }


        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string CustomerName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string CustomerSurname { get; set; } = string.Empty;


        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters.")]
        public string CustomerMiddleName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string CustomerEmail { get; set; } = string.Empty;


        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(12, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string CustomerPhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;
    }

}
