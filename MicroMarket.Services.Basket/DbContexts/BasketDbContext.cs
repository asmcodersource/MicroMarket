using MicroMarket.Services.Basket.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Basket.DbContexts
{
    public class BasketDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Item> Items { get; set; }
        public DbSet<Product> Products { get; set; }


        public BasketDbContext(DbContextOptions<BasketDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_configuration["ConnectionString"] is not null)
                optionsBuilder.UseNpgsql(_configuration["ConnectionString"]);
            else if (EF.IsDesignTime)
                optionsBuilder.UseNpgsql();
            else
                throw new InvalidOperationException();
        }
    }
}
