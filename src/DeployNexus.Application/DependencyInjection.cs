using DeployNexus.Application.Users.Interfaces;
using DeployNexus.Application.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}