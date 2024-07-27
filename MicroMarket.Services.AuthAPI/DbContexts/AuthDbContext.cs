using Microsoft.EntityFrameworkCore;
using MicroMarket.Services.AuthAPI.Models;

namespace MicroMarket.Services.AuthAPI.DbContexts
{
    public class AuthDbContext: DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationUserRole> ApplicationRoles { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options, IConfiguration configuration) 
            : base(options) 
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
