using MicroMarket.Services.AuthAPI.DbContexts;
using MicroMarket.Services.AuthAPI.Models;
using MicroMarket.Services.AuthAPI.Utilities;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(Authorization.GetTokenValidationParameters(builder.Configuration));
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddConfiguredIdentityCore<AuthDbContext>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
