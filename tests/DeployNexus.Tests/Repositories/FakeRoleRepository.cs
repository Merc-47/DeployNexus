using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Tests.Roles;

public class FakeRoleRepository : IRoleRepository
{
    private readonly List<Role> _roles = new();

    public Task<Role> AddAsync(Role role)
    {
        _roles.Add(role);

        return Task.FromResult(role);
    }

    public Task<Role?> GetByIdAsync(Guid id)
    {
        var role = _roles
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult(role);
    }

    public Task<IEnumerable<Role>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Role>>(
            _roles);
    }

    public Task<bool> ExistsByNameAsync(
        Guid organizationId,
        string name)
    {
        var exists = _roles.Any(x =>
            x.OrganizationId == organizationId &&
            x.Name == name);

        return Task.FromResult(exists);
    }

    public Task UpdateAsync(Role role)
    {
        var existing = _roles
            .FirstOrDefault(x => x.Id == role.Id);

        if (existing != null)
        {
            existing.Name = role.Name;
            existing.Description = role.Description;
            existing.IsActive = role.IsActive;
            existing.OrganizationId = role.OrganizationId;
        }

        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}