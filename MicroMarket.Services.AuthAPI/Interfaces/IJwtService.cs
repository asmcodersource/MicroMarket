using MicroMarket.Services.AuthAPI.Models;
using System.IdentityModel.Tokens.Jwt;

using CSharpFunctionalExtensions;

namespace MicroMarket.Services.AuthAPI.Interfaces
{
    public interface IJwtService
    {
        public Task<Result<JwtSecurityToken>> CreateUserJwtToken(ApplicationUser user);
        public Task<Result> RevokeUserJwtToken(ApplicationUser user);
        public string EncodeToken(JwtSecurityToken jwtSecurityToken);
    }
}
