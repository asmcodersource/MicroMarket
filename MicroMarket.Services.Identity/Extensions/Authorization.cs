using MicroMarket.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace MicroMarket.Services.Identity.Extensions
{
    public static class Authorization
    {
        public static void AddConfiguredIdentityCore<ConreteDbContext>(this IServiceCollection services)
            where ConreteDbContext : DbContext
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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
    }
}
