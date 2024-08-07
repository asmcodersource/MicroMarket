using MicroMarket.Services.SharedCore.SharedRedis.Extensions;
using MicroMarket.Services.SharedCore.TokenValidation.Extensions;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("ocelot.json", true, true);
builder.Services.AddSharedRedisDistributedCache(builder.Configuration);
builder.Services.AddTokenValidation();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MicroMarket API", Version = "v1" });
});
builder.Services.AddOcelot();
var app = builder.Build();
app.UseRouting();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}
app.UseHttpsRedirection();
app.UseSwaggerForOcelotUI();
app.UseTokenValidation();
app.UseStaticFiles();
await app.UseOcelot();
await app.RunAsync();
