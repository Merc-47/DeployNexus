using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Tests.Repositories;

public class FakePermissionRepository : IPermissionRepository
{
    private readonly List<Permission> _permissions = new();

    private readonly Dictionary<(Guid UserId, string PermissionCode), bool>
        _userPermissions = new();


    public Task<Permission> AddAsync(Permission permission)
    {
        _permissions.Add(permission);

        return Task.FromResult(permission);
    }


    public Task<Permission?> GetByIdAsync(Guid id)
    {
        var permission = _permissions
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult(permission);
    }


    public Task<IEnumerable<Permission>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Permission>>(
            _permissions);
    }


    public Task UpdateAsync(Permission permission)
    {
        var existing = _permissions
            .FirstOrDefault(x => x.Id == permission.Id);

        if (existing != null)
        {
            existing.Name = permission.Name;
            existing.Code = permission.Code;
            existing.IsActive = permission.IsActive;
        }

        return Task.CompletedTask;
    }


    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }


    public Task<bool> UserHasPermissionAsync(
        Guid userId,
        string permissionCode)
    {
        var result = _userPermissions.TryGetValue(
            (userId, permissionCode),
            out var hasPermission)
            && hasPermission;

        return Task.FromResult(result);
    }


    public void SetUserPermission(
        Guid userId,
        string permissionCode,
        bool hasPermission)
    {
        _userPermissions[(userId, permissionCode)]
            = hasPermission;
    }
}