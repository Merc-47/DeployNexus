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

    public DbSet<Module> Modules => Set<Module>();


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // ============================================================
        // Role → Organization
        // ============================================================

        modelBuilder.Entity<Role>()
            .HasOne(r => r.Organization)
            .WithMany(o => o.Roles)
            .HasForeignKey(r => r.OrganizationId);


        modelBuilder.Entity<Role>()
            .HasIndex(r => new
            {
                r.OrganizationId,
                r.Name
            })
            .IsUnique();


        // ============================================================
        // RolePermission → Role
        // ============================================================

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);


        // ============================================================
        // RolePermission → Permission
        // ============================================================

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);


        modelBuilder.Entity<RolePermission>()
            .HasIndex(rp => new
            {
                rp.RoleId,
                rp.PermissionId
            })
            .IsUnique();


        // ============================================================
        // Module
        // ============================================================

        modelBuilder.Entity<Module>()
            .HasIndex(m => m.Code)
            .IsUnique();


        // ============================================================
        // Module → Permission
        // ============================================================

        modelBuilder.Entity<Permission>()
            .HasOne(p => p.Module)
            .WithMany(m => m.Permissions)
            .HasForeignKey(p => p.ModuleId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Code)
            .IsUnique();


        // ============================================================
        // User → Role
        // ============================================================

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);


        // ============================================================
        // User → Organization-scoped username uniqueness
        // ============================================================

        modelBuilder.Entity<User>()
            .HasIndex(u => new
            {
                u.OrganizationId,
                u.Username
            })
            .IsUnique();


        // ============================================================
        // User → Organization-scoped email uniqueness
        // ============================================================

        modelBuilder.Entity<User>()
            .HasIndex(u => new
            {
                u.OrganizationId,
                u.Email
            })
            .IsUnique();
    }
}