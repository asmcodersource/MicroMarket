using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using MicroMarker.Services.SharedCore.TokenValidation.Middlewares;
using MicroMarker.Services.SharedCore.TokenValidation.Services;

namespace MicroMarker.Services.SharedCore.TokenValidation.Extensions
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
