using DeployNexus.Infrastructure.Data;
using DeployNexus.Infrastructure.Repositories;
using DeployNexus.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<DeployNexusDbContext>(options =>
            options.UseSqlServer(connectionString));


        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<
            IOrganizationRepository,
            OrganizationRepository>();


        return services;
    }
}