using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Common.Interfaces;

public interface IRoleRepository
{
    // ============================================================
    // CREATE
    // ============================================================

    Task<Role> AddAsync(Role role);


    // ============================================================
    // GET BY ID - SYSTEM ACCESS
    // ============================================================

    Task<Role?> GetByIdAsync(
        Guid id);


    // ============================================================
    // GET BY ID - ORGANIZATION SCOPED
    // ============================================================

    Task<Role?> GetByIdAsync(
        Guid id,
        Guid organizationId);


    // ============================================================
    // GET ALL - SYSTEM ACCESS
    // ============================================================

    Task<IEnumerable<Role>> GetAllAsync();


    // ============================================================
    // GET ALL - ORGANIZATION SCOPED
    // ============================================================

    Task<IEnumerable<Role>> GetAllAsync(
        Guid organizationId);


    // ============================================================
    // CHECK ROLE NAME
    // ============================================================

    Task<bool> ExistsByNameAsync(
        Guid organizationId,
        string name);


    // ============================================================
    // UPDATE
    // ============================================================

    Task UpdateAsync(
        Role role);


    // ============================================================
    // SAVE
    // ============================================================

    Task SaveChangesAsync();
}