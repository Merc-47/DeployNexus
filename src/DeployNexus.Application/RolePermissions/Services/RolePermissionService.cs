using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.RolePermissions.DTOs;
using DeployNexus.Application.RolePermissions.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.RolePermissions.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;


    public RolePermissionService(
        IRolePermissionRepository rolePermissionRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }


    public async Task<RolePermissionDto> AssignAsync(
        Guid roleId,
        AssignPermissionRequest request)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);

        if (role == null)
        {
            throw new Exception("Role not found");
        }


        var permission = await _permissionRepository
            .GetByIdAsync(request.PermissionId);

        if (permission == null)
        {
            throw new Exception("Permission not found");
        }


        var existing = await _rolePermissionRepository
            .GetAsync(roleId, request.PermissionId);


        if (existing != null)
        {
            throw new Exception("Permission already assigned");
        }


        var rolePermission = new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            PermissionId = request.PermissionId
        };


        await _rolePermissionRepository
            .AddAsync(rolePermission);

        await _rolePermissionRepository
            .SaveChangesAsync();


        return new RolePermissionDto
        {
            RoleId = roleId,
            PermissionId = permission.Id,
            PermissionName = permission.Name,
            PermissionCode = permission.Code
        };
    }



    public async Task<bool> RemoveAsync(
        Guid roleId,
        Guid permissionId)
    {
        var rolePermission =
            await _rolePermissionRepository
                .GetAsync(roleId, permissionId);


        if (rolePermission == null)
        {
            return false;
        }


        await _rolePermissionRepository
            .RemoveAsync(rolePermission);


        await _rolePermissionRepository
            .SaveChangesAsync();


        return true;
    }



    public async Task<IEnumerable<RolePermissionDto>> GetPermissionsAsync(
        Guid roleId)
    {
        var rolePermissions =
            await _rolePermissionRepository
                .GetPermissionsForRoleAsync(roleId);


        return rolePermissions.Select(x => new RolePermissionDto
        {
            RoleId = x.RoleId,
            PermissionId = x.PermissionId,
            PermissionName = x.Permission.Name,
            PermissionCode = x.Permission.Code
        });
    }
}