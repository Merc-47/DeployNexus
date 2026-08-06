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

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>()
            .HasOne(r => r.Organization)
            .WithMany(o => o.Roles)
            .HasForeignKey(r => r.OrganizationId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);
    }
}