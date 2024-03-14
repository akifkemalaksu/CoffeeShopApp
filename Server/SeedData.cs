using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using System.Security.Claims;

namespace Server
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddDbContext<AspNetIdentityDbContext>(
                options => options.UseSqlServer(connectionString));

            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AspNetIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddOperationalDbContext(
                options => options.ConfigureDbContext = dbBuilder =>
                    dbBuilder.UseSqlServer(connectionString,
                        sqlServerOptions => sqlServerOptions.MigrationsAssembly(typeof(SeedData).Assembly.FullName)));


            services.AddConfigurationDbContext(
                options => options.ConfigureDbContext = dbBuilder =>
                    dbBuilder.UseSqlServer(connectionString,
                        sqlServerOptions => sqlServerOptions.MigrationsAssembly(typeof(SeedData).Assembly.FullName)));

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var persistedGrantDbContext = scope.ServiceProvider.GetService<PersistedGrantDbContext>();
            persistedGrantDbContext.Database.Migrate();

            var configurationDbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
            configurationDbContext.Database.Migrate();

            EnsureSeedData(configurationDbContext);

            var aspNetIdentityDbContext = scope.ServiceProvider.GetService<AspNetIdentityDbContext>();
            aspNetIdentityDbContext.Database.Migrate();

            EnsureUsers(scope);
        }

        private static void EnsureUsers(IServiceScope scope)
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var angella = userManager.FindByNameAsync("angella").Result;
            if (angella == null)
            {
                angella = new IdentityUser
                {
                    UserName = "angella",
                    Email = "angella.freeman@email.com",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(angella, "Pass123$").Result;

                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                result = userManager.AddClaimsAsync(
                    angella,
                    new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Angella Freeman"),
                        new Claim(JwtClaimTypes.GivenName, "Angella"),
                        new Claim(JwtClaimTypes.FamilyName, "Freeman"),
                        new Claim(JwtClaimTypes.WebSite, "http://angellafreeman.com"),
                        new Claim("location","somewhere")
                    }).Result;

                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);
            }
        }

        private static void EnsureSeedData(ConfigurationDbContext configurationDbContext)
        {
            if (!configurationDbContext.Clients.Any())
            {
                foreach (var client in Config.Clients.ToList())
                    configurationDbContext.Clients.Add(client.ToEntity());

                configurationDbContext.SaveChanges();
            }

            if (!configurationDbContext.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources.ToList())
                    configurationDbContext.IdentityResources.Add(resource.ToEntity());

                configurationDbContext.SaveChanges();
            }

            if (!configurationDbContext.ApiScopes.Any())
            {
                foreach (var scope in Config.ApiScopes.ToList())
                    configurationDbContext.ApiScopes.Add(scope.ToEntity());

                configurationDbContext.SaveChanges();
            }

            if (!configurationDbContext.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources.ToList())
                    configurationDbContext.ApiResources.Add(resource.ToEntity());

                configurationDbContext.SaveChanges();
            }
        }
    }
}
