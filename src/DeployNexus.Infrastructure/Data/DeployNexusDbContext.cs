using DeployNexus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Infrastructure.Data;

public class DeployNexusDbContext : DbContext
{
    public DeployNexusDbContext(
        DbContextOptions<DeployNexusDbContext> options)
        : base(options)
    {
    }


    public DbSet<User> Users { get; set; }

    public DbSet<Organization> Organizations { get; set; }
}