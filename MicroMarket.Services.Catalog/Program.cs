using MicroMarket.Services.Catalog.DbContexts;
using MicroMarket.Services.Catalog.Interfaces;
using MicroMarket.Services.Catalog.Services;
using MicroMarket.Services.SharedCore.Extensions;
using MicroMarket.Services.SharedCore.MessageBus.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);
if (!EF.IsDesignTime)
{
    // Add services to the container.
    builder.Services.AddDbContext<CatalogDbContext>();
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddScoped<ICategoriesService, CategoriesService>();
    builder.Services.AddScoped<IProductsService, ProductsService>();
    builder.Services.AddSingleton<CatalogMessagingService>();
    builder.Services.AddControllers().AddJsonOptions(opts =>
    {
        var enumConverter = new JsonStringEnumConverter();
        opts.JsonSerializerOptions.Converters.Add(enumConverter);
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddConfiguredSwaggerGen("MicroMarket.Service.Catalog API");
    builder.Services.AddConfiguratedAuthentication(builder.Configuration);
    builder.Services.AddMessageBusService();
    builder.Services.AddAuthorization();
}
else
{
    builder.Services.AddDbContext<CatalogDbContext>();
}
var app = builder.Build();

var singletonService = app.Services.GetRequiredService<CatalogMessagingService>();

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
