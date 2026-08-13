using DeployNexus.Application.Common;
using DeployNexus.Application.Permissions.DTOs;
using DeployNexus.Application.Permissions.Interfaces;

namespace DeployNexus.Tests.Integration;

public class FakePermissionService : IPermissionService
{
    private readonly Dictionary<Guid, HashSet<string>>
        _permissions = new();

    public void SetPermission(
        Guid userId,
        string permissionCode,
        bool hasPermission)
    {
        if (!_permissions.TryGetValue(
                userId,
                out var permissions))
        {
            permissions = new HashSet<string>();
            _permissions[userId] = permissions;
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
            _permissions.TryGetValue(
                userId,
                out var permissions)
            && permissions.Contains(permissionCode);

        return Task.FromResult(hasPermission);
    }


    public Task<PermissionDto> CreateAsync(
        CreatePermissionRequest request)
    {
        throw new NotImplementedException();
    }


    public Task<PermissionDto?> GetByIdAsync(
        Guid id)
    {
        throw new NotImplementedException();
    }


    public Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }


    public Task<PermissionDto?> UpdateAsync(
        Guid id,
        UpdatePermissionRequest request)
    {
        throw new NotImplementedException();
    }


    public Task<Result> DeactivateAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}