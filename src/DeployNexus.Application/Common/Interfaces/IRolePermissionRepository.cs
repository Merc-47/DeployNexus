using DeployNexus.Domain.Entities;

public interface IRolePermissionRepository
{
    Task AddAsync(RolePermission rolePermission);

    Task RemoveAsync(RolePermission rolePermission);

    Task<RolePermission?> GetAsync(Guid roleId, Guid permissionId);

    Task<IEnumerable<RolePermission>> GetPermissionsForRoleAsync(Guid roleId);

    Task SaveChangesAsync();
}