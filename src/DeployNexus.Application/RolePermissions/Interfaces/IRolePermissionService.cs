using DeployNexus.Application.RolePermissions.DTOs;

namespace DeployNexus.Application.RolePermissions.Interfaces;

public interface IRolePermissionService
{
    Task<RolePermissionDto> AssignAsync(
        Guid roleId,
        AssignPermissionRequest request);


    Task<bool> RemoveAsync(
        Guid roleId,
        Guid permissionId);


    Task<IEnumerable<RolePermissionDto>> GetPermissionsAsync(
        Guid roleId);
}