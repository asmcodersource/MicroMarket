using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MicroMarket.Services.SharedCore.Extensions
{
    public static class Authorization
    {
        public static void AddConfiguratedAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenValidationParameters = GetTokenValidationParameters(configuration);
            services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = tokenValidationParameters.ValidIssuer,
                        ValidateAudience = true,
                        ValidAudience = tokenValidationParameters.ValidAudience,
                        ValidateLifetime = true,
                        IssuerSigningKey = tokenValidationParameters.IssuerSigningKey,
                        ValidateIssuerSigningKey = true
                    };
                });
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
