using DeployNexus.Application.Common;
using DeployNexus.Application.Permissions.DTOs;
using DeployNexus.Application.Permissions.Interfaces;

namespace DeployNexus.Tests.Integration;

public class FakePermissionService : IPermissionService
{
    private readonly Dictionary<Guid, PermissionDto> _permissions = new();

    private readonly Dictionary<Guid, HashSet<string>>
        _userPermissions = new();


    // ============================================================
    // TEST PERMISSION AUTHORIZATION
    // ============================================================

    public void SetPermission(
        Guid userId,
        string permissionCode,
        bool hasPermission)
    {
        if (!_userPermissions.TryGetValue(
                userId,
                out var permissions))
        {
            permissions = new HashSet<string>();
            _userPermissions[userId] = permissions;
        }

        if (hasPermission)
        {
            permissions.Add(permissionCode);
        }
        else
        {
            permissions.Remove(permissionCode);
        }
    }


    public Task<bool> HasPermissionAsync(
        Guid userId,
        string permissionCode)
    {
        var hasPermission =
            _userPermissions.TryGetValue(
                userId,
                out var permissions)
            && permissions.Contains(permissionCode);

        return Task.FromResult(hasPermission);
    }


    // ============================================================
    // CREATE
    // ============================================================

    public Task<PermissionDto> CreateAsync(
        CreatePermissionRequest request)
    {
        var permission = new PermissionDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            IsActive = true
        };

        _permissions[permission.Id] = permission;

        return Task.FromResult(permission);
    }


    // ============================================================
    // GET BY ID
    // ============================================================

    public Task<PermissionDto?> GetByIdAsync(
        Guid id)
    {
        _permissions.TryGetValue(
            id,
            out var permission);

        return Task.FromResult(permission);
    }


    // ============================================================
    // GET ALL
    // ============================================================

    public Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<PermissionDto>>(
            _permissions.Values.ToList());
    }


    // ============================================================
    // UPDATE
    // ============================================================

    public Task<PermissionDto?> UpdateAsync(
        Guid id,
        UpdatePermissionRequest request)
    {
        if (!_permissions.TryGetValue(
                id,
                out var permission))
        {
            return Task.FromResult<PermissionDto?>(
                null);
        }

        permission.Name = request.Name;
        permission.Code = request.Code;

        return Task.FromResult<PermissionDto?>(
            permission);
    }


    // ============================================================
    // DEACTIVATE
    // ============================================================

    public Task<Result> DeactivateAsync(Guid id)
    {
        if (!_permissions.TryGetValue(
                id,
                out var permission))
        {
            return Task.FromResult(
                Result.Failure("Permission not found"));
        }

        permission.IsActive = false;

        return Task.FromResult(
            Result.Ok());
    }
}