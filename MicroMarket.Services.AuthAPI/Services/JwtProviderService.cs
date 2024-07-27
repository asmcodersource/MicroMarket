using MicroMarket.Services.AuthAPI.Interfaces;
using MicroMarket.Services.AuthAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MicroMarket.Services.AuthAPI.Services
{
    public class JwtProviderService : IJwtProviderService
    {
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtProviderService(TokenValidationParameters tokenValidationParameters)
        {
            _tokenValidationParameters = tokenValidationParameters;
        }

        public JwtSecurityToken CreateJwtToken(ApplicationUser user, IList<String> roles)
        {
            var claims = new List<Claim> {
                new Claim("Id", user.Id),
                new Claim("Email", user.Email!),
                new Claim("Name", user.Name),
                new Claim("Surname", user.Name),
                new Claim("MiddleName", user.Name)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return new JwtSecurityToken(
                issuer: _tokenValidationParameters.ValidIssuer,
                audience: _tokenValidationParameters.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(7)),
                signingCredentials: new SigningCredentials(_tokenValidationParameters.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

        public string EncodeToken(JwtSecurityToken jwtSecurityToken)
        {
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
