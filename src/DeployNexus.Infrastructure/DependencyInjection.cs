using DeployNexus.Application.Users.Interfaces;
using DeployNexus.Application.Users.Services;
using DeployNexus.Application.Organizations.Interfaces;
using DeployNexus.Application.Organizations.Services;
using DeployNexus.Application.Roles.Interfaces;
using DeployNexus.Application.Roles.Services;
using Microsoft.Extensions.DependencyInjection;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Infrastructure.Data;
using DeployNexus.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    string connectionString)
    {
        services.AddDbContext<DeployNexusDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IOrganizationRepository, OrganizationRepository>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<IPermissionRepository, PermissionRepository>();

        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        return services;
    }
}