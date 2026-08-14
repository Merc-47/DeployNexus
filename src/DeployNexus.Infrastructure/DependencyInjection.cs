using DeployNexus.Application.Authentication.Interfaces;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Infrastructure.Authentication;
using DeployNexus.Infrastructure.Data;
using DeployNexus.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration)
    {
        // ============================================================
        // Database
        // ============================================================

        services.AddDbContext<DeployNexusDbContext>(options =>
            options.UseSqlServer(connectionString));


        // ============================================================
        // JWT Configuration
        // ============================================================

        services.Configure<JwtSettings>(
            configuration.GetSection("Jwt"));


        // ============================================================
        // Repositories
        // ============================================================

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IOrganizationRepository, OrganizationRepository>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<IPermissionRepository, PermissionRepository>();

        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();


        // ============================================================
        // Authentication Infrastructure
        // ============================================================

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


        return services;
    }
}