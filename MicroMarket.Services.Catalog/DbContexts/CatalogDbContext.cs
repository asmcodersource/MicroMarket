using MicroMarket.Services.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Catalog.DbContexts
{
    public class CatalogDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OutboxOperation> OutboxOperations { get; set; }
        public DbSet<ItemsClaimOperation> ItemsClaimOperations { get; set; }

        public CatalogDbContext(DbContextOptions<CatalogDbContext> options, IConfiguration configuration)
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
