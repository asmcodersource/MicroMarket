using MicroMarket.Services.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Identity.DbContexts
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configuration["ConnectionString"] is not null)
                optionsBuilder.UseNpgsql(_configuration["ConnectionString"]);
            else
                optionsBuilder.UseNpgsql();
        }
    }
}
