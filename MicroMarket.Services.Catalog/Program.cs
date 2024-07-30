using MicroMarket.Services.SharedCore.SharedRedis.Extensions;
using MicroMarket.Services.SharedCore.TokenValidation.Extensions;
using MicroMarket.Services.Catalog.Extensions;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddConfiguredSwaggerGen();
builder.Services.AddConfiguratedAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddTokenValidation();
builder.Services.AddSharedRedisDistributedCache(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseTokenValidation();
app.UseHttpsRedirection();
app.MapControllers();
await app.RunAsync();
