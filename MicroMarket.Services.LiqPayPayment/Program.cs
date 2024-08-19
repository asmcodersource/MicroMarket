using MicroMarket.Services.SharedCore.Extensions;
using MicroMarket.Services.SharedCore.MessageBus.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);
if (!EF.IsDesignTime)
{
    // Add services to the container.
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddControllers().AddJsonOptions(opts =>
    {
        var enumConverter = new JsonStringEnumConverter();
        opts.JsonSerializerOptions.Converters.Add(enumConverter);
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddConfiguredSwaggerGen("MicroMarket.Service.LiqPayPayment API");
    builder.Services.AddConfiguratedAuthentication(builder.Configuration);
    builder.Services.AddMessageBusService();
    builder.Services.AddAuthorization();
}
else
{
    //builder.Services.AddDbContext<OrderingDbContext>();
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
            //var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
            //await dbContext.Database.MigrateAsync();
        }
    }
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
await app.RunAsync();
