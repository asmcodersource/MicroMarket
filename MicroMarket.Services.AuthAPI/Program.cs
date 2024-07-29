using MicroMarket.Services.AuthAPI.DbContexts;
using MicroMarket.Services.AuthAPI.Models;
using MicroMarket.Services.AuthAPI.Utilities;
using MicroMarket.Services.AuthAPI.Interfaces;
using MicroMarket.Services.AuthAPI.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MicroMarker.Services.SharedCore.TokenValidation.Extensions;
using MicroMarker.Services.SharedCore.SharedRedis.Extensions;
using System.Text;


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
app.UseHttpsRedirection();
app.MapControllers();
await app.RunAsync();
