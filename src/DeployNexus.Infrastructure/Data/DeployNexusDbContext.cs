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

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>()
            .HasOne(r => r.Organization)
            .WithMany(o => o.Roles)
            .HasForeignKey(r => r.OrganizationId);
    }
}