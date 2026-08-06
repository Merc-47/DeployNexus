using DeployNexus.Application.Users.Interfaces;
using DeployNexus.Application.Users.Services;
using DeployNexus.Application.Organizations.Interfaces;
using DeployNexus.Application.Organizations.Services;
using Microsoft.Extensions.DependencyInjection;
using DeployNexus.Application.Roles.Interfaces;
using DeployNexus.Application.Roles.Services;
using DeployNexus.Application.Permissions.Interfaces;
using DeployNexus.Application.Permissions.Services;
using DeployNexus.Application.RolePermissions.Interfaces;
using DeployNexus.Application.RolePermissions.Services;

namespace DeployNexus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IOrganizationService, OrganizationService>();

        services.AddScoped<IRoleService, RoleService>();

        services.AddScoped<IPermissionService, PermissionService>();

        services.AddScoped<IRolePermissionService, RolePermissionService>();

        return services;
    }
}