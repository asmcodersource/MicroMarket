using CSharpFunctionalExtensions;
using MicroMarket.Services.Identity.Models;
using System.IdentityModel.Tokens.Jwt;

namespace MicroMarket.Services.Identity.Interfaces
{
    public interface IJwtService
    {
        public Task<Result<JwtSecurityToken>> CreateUserJwtToken(ApplicationUser user);
        public Task<Result> RevokeUserJwtToken(ApplicationUser user);
        public string EncodeToken(JwtSecurityToken jwtSecurityToken);
    }
}
