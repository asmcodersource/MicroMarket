using CSharpFunctionalExtensions;
using MicroMarket.Services.Identity.Interfaces;
using MicroMarket.Services.Identity.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MicroMarket.Services.Identity.Services
{
    public class JwtService : IJwtService
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IDistributedCache _distributedCache;
        private readonly IRolesService _rolesService;
        private readonly int _revokeTimeMinutes;
        private readonly byte[] _key;

        public JwtService(TokenValidationParameters tokenValidationParameters, IRolesService rolesService, IDistributedCache distributedCache, IConfiguration configuration)
        {
            var jwtOptionsSection = configuration.GetSection("ApiSettings:JwtOptions");
            _key = Encoding.ASCII.GetBytes(jwtOptionsSection["Key"]);
            _revokeTimeMinutes = Convert.ToInt32(jwtOptionsSection["TokenRovokeTimeMinutes"]);
            _rolesService = rolesService;
            _distributedCache = distributedCache;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<Result<JwtSecurityToken>> CreateUserJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim> {
                new Claim("Id", user.Id),
                new Claim("Email", user.Email!),
                new Claim("Name", user.Name),
                new Claim("Surname", user.Name),
                new Claim("MiddleName", user.Name)
            };

            var rolesResult = await _rolesService.GetRoles(user.Id);
            if (rolesResult.IsFailure)
                return Result.Failure<JwtSecurityToken>($"Error getting user roles when creating JWT token: {rolesResult.Error}");
            claims.AddRange(rolesResult.Value.Select(role => new Claim(ClaimTypes.Role, role.ToUpperInvariant())));

            return new JwtSecurityToken(
                issuer: _tokenValidationParameters.ValidIssuer,
                audience: _tokenValidationParameters.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_revokeTimeMinutes)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256)
            );
        }

        public async Task<Result> RevokeUserJwtToken(ApplicationUser user)
        {
            var jwtTokenToRevokeResult = await CreateUserJwtToken(user);
            if (jwtTokenToRevokeResult.IsFailure)
                return Result.Failure($"Error getting cancellation token for user: {jwtTokenToRevokeResult.Error}");
            var encodedJwtToken = EncodeToken(jwtTokenToRevokeResult.Value);
            await _distributedCache.SetAsync(
                encodedJwtToken,
                Array.Empty<byte>(),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_revokeTimeMinutes)
                }
            );
            return Result.Success();
        }

        public string EncodeToken(JwtSecurityToken jwtSecurityToken)
        {
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
