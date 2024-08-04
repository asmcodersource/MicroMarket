using MicroMarket.Services.SharedCore.SharedRedis.Extensions;
using MicroMarket.Services.SharedCore.TokenValidation.Extensions;
using MicroMarket.Services.Catalog.Extensions;
using MicroMarket.Services.Catalog.DbContexts;
using Microsoft.EntityFrameworkCore;
using MicroMarket.Services.Catalog.Interfaces;
using MicroMarket.Services.Catalog.Services;
using MicroMarket.Services.Catalog.Models;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);
if (!EF.IsDesignTime)
{
    // Add services to the container.
    builder.Services.AddDbContext<CatalogDbContext>();
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddScoped<ICategoriesService, CategoriesService>();
    builder.Services.AddScoped<IProductsService, ProductsService>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddConfiguredSwaggerGen();
    builder.Services.AddConfiguratedAuthentication(builder.Configuration);
    builder.Services.AddAuthorization();
} else
{
    builder.Services.AddDbContext<CatalogDbContext>();
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    if (!EF.IsDesignTime)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            await dbContext.Database.MigrateAsync();
            await CatalogDataSeeder.SeedData(dbContext);
        }
    }
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
await app.RunAsync();
