using MicroMarket.Services.SharedCore.SharedRedis.Extensions;
using MicroMarket.Services.SharedCore.TokenValidation.Extensions;
using MicroMarket.Services.Identity.DbContexts;
using MicroMarket.Services.Identity.Interfaces;
using MicroMarket.Services.Identity.Services;
using MicroMarket.Services.Identity.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MicroMarket.Services.Identity.Models;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
if (!EF.IsDesignTime)
{
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddSingleton(Authorization.GetTokenValidationParameters(builder.Configuration));
    builder.Services.AddDbContext<IdentityDbContext>();
    builder.Services.AddConfiguredIdentityCore<IdentityDbContext>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IRolesService, RolesService>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddConfiguredSwaggerGen();
    builder.Services.AddConfiguratedAuthentication(builder.Configuration);
    builder.Services.AddAuthorization();
    builder.Services.AddSharedRedisDistributedCache(builder.Configuration);
} else
{
    builder.Services.AddDbContext<IdentityDbContext>();
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
            var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await dbContext.Database.MigrateAsync();
            await IdentityDataSeeder.SeedData(dbContext, userManager, roleManager);
        }
    }
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
