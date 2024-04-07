using Client.Components;
using Client.Components.Pages;
using Client.Services;
using Client.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var interactiveServiceSettings = builder.Configuration.GetSection(nameof(InteractiveServiceSettings)).Get<InteractiveServiceSettings>();
var serviceUrlSettings = builder.Configuration.GetSection(nameof(ServiceUrlSettings)).Get<ServiceUrlSettings>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
    options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        options.Authority = serviceUrlSettings.IdentityServerUrl;
        options.ClientId = interactiveServiceSettings.ClientId;
        options.ClientSecret = interactiveServiceSettings.ClientSecret;

        options.ResponseType = "code";
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
    });

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<ServiceUrlSettings>(builder.Configuration.GetSection(nameof(ServiceUrlSettings)));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ServiceUrlSettings>>().Value);

builder.Services.Configure<IdentityServerSettings>(builder.Configuration.GetSection(nameof(IdentityServerSettings)));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<IdentityServerSettings>>().Value);

builder.Services.AddHttpClient<ITokenService, TokenService>();

builder.Services.AddHttpClient<CoffeeShops>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
