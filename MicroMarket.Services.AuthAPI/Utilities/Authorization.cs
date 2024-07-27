using MicroMarket.Services.AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace MicroMarket.Services.AuthAPI.Utilities
{
    public static class Authorization
    {
        public static void AddConfiguredIdentityCore<ConreteDbContext>(this IServiceCollection services)
            where ConreteDbContext : DbContext
        {
            services.AddIdentity<ApplicationUser, ApplicationUserRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ConreteDbContext>()
                .AddDefaultTokenProviders();
        }

        public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
        {
            var jwtOptionsSection = configuration.GetSection("ApiSettings:JwtOptions");
            var key = jwtOptionsSection["Key"];
            if (key is null)
                throw new InvalidOperationException("Jwt key have to be provided");
                    
            return new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = jwtOptionsSection["Audience"],
                ValidateIssuer = true,
                ValidIssuer = jwtOptionsSection["Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            };
        }
    }
}
