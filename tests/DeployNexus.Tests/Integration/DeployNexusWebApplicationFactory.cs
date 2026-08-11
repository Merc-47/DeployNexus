using DeployNexus.Application.Permissions.Interfaces;
using DeployNexus.Application.Roles.Interfaces;
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
            // Replace normal authentication
            // with test authentication.
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


            // Remove the real permission service.
            var permissionServiceDescriptor =
                services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(IPermissionService));

            if (permissionServiceDescriptor != null)
            {
                services.Remove(
                    permissionServiceDescriptor);
            }

            // Fake Role Service

            services.AddSingleton<FakeRoleService>();

            services.AddSingleton<IRoleService>(sp =>
                sp.GetRequiredService<FakeRoleService>());

            // Fake Permission Service

            services.AddSingleton<FakePermissionService>();

            services.AddSingleton<IPermissionService>(sp =>
                sp.GetRequiredService<FakePermissionService>());
        });
    }
}