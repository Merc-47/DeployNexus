using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Tests.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Tests.Integration;

public class JwtWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Fake User Repository

            RemoveService<IUserRepository>(services);

            services.AddSingleton<FakeUserRepository>();

            services.AddSingleton<IUserRepository>(sp =>
                sp.GetRequiredService<FakeUserRepository>());

            // Use the real JWT authentication scheme

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                });
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