using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Common.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization> AddAsync(Organization organization);

    Task<Organization?> GetByIdAsync(Guid id);

    Task<IEnumerable<Organization>> GetAllAsync();

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeOrganizationId = null);

    Task UpdateAsync(Organization organization);

    Task SaveChangesAsync();
}