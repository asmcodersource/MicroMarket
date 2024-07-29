using Microsoft.EntityFrameworkCore;
using MicroMarket.Services.AuthAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MicroMarket.Services.AuthAPI.DbContexts
{
    public class AuthDbContext: IdentityDbContext<ApplicationUser>
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
