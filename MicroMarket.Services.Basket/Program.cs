using MicroMarket.Services.Basket.DbContexts;
using MicroMarket.Services.Basket.Interfaces;
using MicroMarket.Services.Basket.Services;
using MicroMarket.Services.SharedCore.Extensions;
using MicroMarket.Services.SharedCore.MessageBus.Extensions;
using Microsoft.EntityFrameworkCore;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);
if (!EF.IsDesignTime)
{
    // Add services to the container.
    builder.Services.AddDbContext<BasketDbContext>();
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddScoped<IBasketService, BasketService>();
    builder.Services.AddSingleton<BasketMessagingService>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddConfiguredSwaggerGen("MicroMarket.Service.Basket API");
    builder.Services.AddConfiguratedAuthentication(builder.Configuration);
    builder.Services.AddMessageBusService();
    builder.Services.AddAuthorization();
}
else
{
    builder.Services.AddDbContext<BasketDbContext>();
}
var app = builder.Build();


var singletonService = app.Services.GetRequiredService<BasketMessagingService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    if (!EF.IsDesignTime)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
await app.RunAsync();
