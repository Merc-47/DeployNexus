using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Tests.Repositories;

public class FakeRolePermissionRepository : IRolePermissionRepository
{
    private readonly List<RolePermission> _rolePermissions = new();


    public Task AddAsync(RolePermission rolePermission)
    {
        _rolePermissions.Add(rolePermission);

        return Task.CompletedTask;
    }


    public Task RemoveAsync(RolePermission rolePermission)
    {
        _rolePermissions.Remove(rolePermission);

        return Task.CompletedTask;
    }


    public Task<RolePermission?> GetAsync(
        Guid roleId,
        Guid permissionId)
    {
        var result = _rolePermissions
            .FirstOrDefault(x =>
                x.RoleId == roleId &&
                x.PermissionId == permissionId);

        return Task.FromResult(result);
    }


    public Task<IEnumerable<RolePermission>> GetPermissionsForRoleAsync(
        Guid roleId)
    {
        var result = _rolePermissions
            .Where(x => x.RoleId == roleId);

        return Task.FromResult<IEnumerable<RolePermission>>(result);
    }


    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}