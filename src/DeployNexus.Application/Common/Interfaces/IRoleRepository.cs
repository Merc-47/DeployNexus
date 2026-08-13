using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Common.Interfaces;

public interface IRoleRepository
{
    Task<Role> AddAsync(Role role);

    Task<Role?> GetByIdAsync(Guid id);

    Task<IEnumerable<Role>> GetAllAsync();

    Task<bool> ExistsByNameAsync(
        Guid organizationId,
        string name);

    Task UpdateAsync(Role role);

    Task SaveChangesAsync();
}