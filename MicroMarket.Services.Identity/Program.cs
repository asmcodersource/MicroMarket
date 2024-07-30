using MicroMarket.Services.SharedCore.SharedRedis.Extensions;
using MicroMarket.Services.SharedCore.TokenValidation.Extensions;
using MicroMarket.Services.Identity.DbContexts;
using MicroMarket.Services.Identity.Interfaces;
using MicroMarket.Services.Identity.Services;
using MicroMarket.Services.Identity.Extensions;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddSingleton(Authorization.GetTokenValidationParameters(builder.Configuration));
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddConfiguredIdentityCore<AuthDbContext>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IJwtService, JwtService>();
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

    if (builder.Configuration["ConnectionString"] is not null)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
app.UseAuthentication();
app.UseAuthorization();
app.UseTokenValidation();
app.MapControllers();
await app.RunAsync();
