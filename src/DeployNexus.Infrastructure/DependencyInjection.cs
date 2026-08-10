using DeployNexus.Application.Users.Interfaces;
using DeployNexus.Application.Users.Services;
using DeployNexus.Application.Organizations.Interfaces;
using DeployNexus.Application.Organizations.Services;
using DeployNexus.Application.Roles.Interfaces;
using DeployNexus.Application.Roles.Services;
using DeployNexus.Application.Authentication.Interfaces;
using DeployNexus.Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Infrastructure.Data;
using DeployNexus.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace DeployNexus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
     this IServiceCollection services,
     string connectionString,
     IConfiguration configuration)
    {
        services.AddDbContext<DeployNexusDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.Configure<JwtSettings>(
            configuration.GetSection("Jwt"));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IOrganizationRepository, OrganizationRepository>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<IPermissionRepository, PermissionRepository>();

        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}