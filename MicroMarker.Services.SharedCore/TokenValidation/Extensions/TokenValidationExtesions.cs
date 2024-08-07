using MicroMarket.Services.SharedCore.TokenValidation.Middlewares;
using MicroMarket.Services.SharedCore.TokenValidation.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MicroMarket.Services.SharedCore.TokenValidation.Extensions
{
    public static class Extensions
    {
        public static void AddTokenValidation(this IServiceCollection services)
        {
            services.AddSingleton<TokenValidationService>();
        }

        public static void UseTokenValidation(this WebApplication app)
        {
            app.UseMiddleware<TokenValidationMiddleware>();
        }
    }
}
