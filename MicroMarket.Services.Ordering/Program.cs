using MicroMarket.Services.Ordering.DbContexts;
using MicroMarket.Services.Ordering.Services;
using MicroMarket.Services.SharedCore.Extensions;
using MicroMarket.Services.SharedCore.MessageBus.Extensions;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
if (!EF.IsDesignTime)
{
    // Add services to the container.
    builder.Services.AddDbContext<OrderingDbContext>();
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddSingleton<OrderingMessagingService>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddConfiguredSwaggerGen("MicroMarket.Service.Ordering API");
    builder.Services.AddConfiguratedAuthentication(builder.Configuration);
    builder.Services.AddMessageBusService();
    builder.Services.AddAuthorization();
}
else
{
    builder.Services.AddDbContext<OrderingDbContext>();
}
var app = builder.Build();


var singletonService = app.Services.GetRequiredService<OrderingMessagingService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    if (!EF.IsDesignTime)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
await app.RunAsync();
