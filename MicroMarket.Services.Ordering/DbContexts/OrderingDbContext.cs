using MicroMarket.Services.Ordering.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Ordering.DbContexts
{
    public class OrderingDbContext: DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Item> Items { get; set; } 
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderState> OrderStates { get; set; }

        public OrderingDbContext(DbContextOptions<OrderingDbContext> options, IConfiguration configuration) : base(options) 
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
