using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.Permissions.Interfaces;
using DeployNexus.Application.Roles.Interfaces;
using DeployNexus.Tests.Repositories;
using DeployNexus.Tests.Roles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Tests.Integration;

public class DeployNexusWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // ============================================================
            // Test Authentication
            // ============================================================

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        TestAuthenticationHandler.SchemeName;

                    options.DefaultChallengeScheme =
                        TestAuthenticationHandler.SchemeName;
                })
                .AddScheme<
                    AuthenticationSchemeOptions,
                    TestAuthenticationHandler>(
                    TestAuthenticationHandler.SchemeName,
                    _ =>
                    {
                    });


            // ============================================================
            // Fake Role Service
            // ============================================================

            RemoveService<IRoleService>(services);

            services.AddSingleton<FakeRoleService>();

            services.AddSingleton<IRoleService>(sp =>
                sp.GetRequiredService<FakeRoleService>());


            // ============================================================
            // Fake Permission Service
            // ============================================================

            RemoveService<IPermissionService>(services);

            services.AddSingleton<FakePermissionService>();

            services.AddSingleton<IPermissionService>(sp =>
                sp.GetRequiredService<FakePermissionService>());


            // ============================================================
            // Fake Role Repository
            // ============================================================

            RemoveService<IRoleRepository>(services);

            services.AddSingleton<FakeRoleRepository>();

            services.AddSingleton<IRoleRepository>(sp =>
                sp.GetRequiredService<FakeRoleRepository>());


            // ============================================================
            // Fake Role Permission Repository
            // ============================================================

            RemoveService<IRolePermissionRepository>(services);

            services.AddSingleton<FakeRolePermissionRepository>();

            services.AddSingleton<IRolePermissionRepository>(sp =>
                sp.GetRequiredService<FakeRolePermissionRepository>());


            // ============================================================
            // Fake Permission Repository
            // ============================================================

            RemoveService<IPermissionRepository>(services);

            services.AddSingleton<FakePermissionRepository>();

            services.AddSingleton<IPermissionRepository>(sp =>
                sp.GetRequiredService<FakePermissionRepository>());
        });
    }


    private static void RemoveService<T>(
        IServiceCollection services)
    {
        var descriptors = services
            .Where(x => x.ServiceType == typeof(T))
            .ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }
}