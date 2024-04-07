using API.Services;
using API.Settings;
using DataAccess.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<ICoffeeShopService, CoffeeShopService>();

var serviceUrlSettings = builder.Configuration.GetSection(nameof(ServiceUrlSettings)).Get<ServiceUrlSettings>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = serviceUrlSettings.IdentityApiUrl;
        options.ApiName = "CoffeeAPI";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("write", policy => policy.RequireScope("CoffeeAPI.write"));
    options.AddPolicy("read", policy => policy.RequireScope("CoffeeAPI.read"));
});


var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

app.Run();
