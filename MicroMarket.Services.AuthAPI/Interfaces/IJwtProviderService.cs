using MicroMarket.Services.AuthAPI.Models;
using System.IdentityModel.Tokens.Jwt;

namespace MicroMarket.Services.AuthAPI.Interfaces
{
    public interface IJwtProviderService
    {
        public JwtSecurityToken CreateJwtToken(ApplicationUser user, IList<String> roles);
        public string EncodeToken(JwtSecurityToken jwtSecurityToken);
    }
}
