using Microsoft.AspNetCore.Identity;

namespace MicroMarket.Services.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
    }
}
