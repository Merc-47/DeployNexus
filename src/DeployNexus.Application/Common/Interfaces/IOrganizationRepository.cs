using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Common.Interfaces;

public interface IOrganizationRepository
{
    // ============================================================
    // SYSTEM ACCESS
    // ============================================================

    Task<Organization> AddAsync(
        Organization organization);

    Task<Organization?> GetByIdAsync(
        Guid id);

    Task<IEnumerable<Organization>> GetAllAsync();


    // ============================================================
    // ORGANIZATION-SCOPED ACCESS
    // ============================================================

    Task<Organization?> GetByIdAsync(
        Guid id,
        Guid organizationId);


    // ============================================================
    // VALIDATION
    // ============================================================

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeOrganizationId = null);


    // ============================================================
    // UPDATE
    // ============================================================

    Task UpdateAsync(
        Organization organization);


    // ============================================================
    // SAVE
    // ============================================================

    Task SaveChangesAsync();
}