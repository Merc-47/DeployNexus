using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Common.Interfaces;

public interface IPermissionRepository
{
    Task<Permission> AddAsync(Permission permission);

    Task<Permission?> GetByIdAsync(Guid id);

    Task<IEnumerable<Permission>> GetAllAsync();

    Task UpdateAsync(Permission permission);

    Task SaveChangesAsync();
}