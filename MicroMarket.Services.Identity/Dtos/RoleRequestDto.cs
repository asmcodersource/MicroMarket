using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Services.Identity.Dtos
{
    public class RoleRequestDto
    {
        [RegularExpression("^[A-Za-z][A-Za-z0-9 _-]*$", ErrorMessage = "Role name is invalid. It must start with a letter and can only contain letters, numbers, spaces, hyphens, and underscores.")]
        public string RoleName { get; set; } = string.Empty;
    }
}
