using DeployNexus.Application.Users.Interfaces;
using DeployNexus.Application.Users.Services;
using DeployNexus.Application.Organizations.Interfaces;
using DeployNexus.Application.Organizations.Services;
using Microsoft.Extensions.DependencyInjection;
using DeployNexus.Application.Roles.Interfaces;
using DeployNexus.Application.Roles.Services;

namespace DeployNexus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IOrganizationService, OrganizationService>();

        services.AddScoped<IRoleService, RoleService>();

        return services;
    }
}