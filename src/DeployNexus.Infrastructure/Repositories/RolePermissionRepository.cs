using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;
using DeployNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Infrastructure.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly DeployNexusDbContext _context;

    public RolePermissionRepository(DeployNexusDbContext context)
    {
        _context = context;
    }


    public async Task AddAsync(RolePermission rolePermission)
    {
        await _context.RolePermissions.AddAsync(rolePermission);
    }


    public Task RemoveAsync(RolePermission rolePermission)
    {
        _context.RolePermissions.Remove(rolePermission);

        return Task.CompletedTask;
    }


    public async Task<RolePermission?> GetAsync(
        Guid roleId,
        Guid permissionId)
    {
        return await _context.RolePermissions
            .FirstOrDefaultAsync(x =>
                x.RoleId == roleId &&
                x.PermissionId == permissionId);
    }


    public async Task<IEnumerable<RolePermission>> GetPermissionsForRoleAsync(
        Guid roleId)
    {
        return await _context.RolePermissions
            .Where(x => x.RoleId == roleId)
            .Include(x => x.Permission)
            .ToListAsync();
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}