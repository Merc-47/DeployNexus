using DeployNexus.Domain.Entities;
using DeployNexus.Infrastructure.Data;
using DeployNexus.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly DeployNexusDbContext _context;


    public OrganizationRepository(
        DeployNexusDbContext context)
    {
        _context = context;
    }


    // ============================================================
    // CREATE
    // ============================================================

    public async Task<Organization> AddAsync(
        Organization organization)
    {
        await _context.Organizations.AddAsync(
            organization);

        return organization;
    }


    // ============================================================
    // GET BY ID - SYSTEM ACCESS
    // ============================================================

    public async Task<Organization?> GetByIdAsync(
        Guid id)
    {
        return await _context.Organizations
            .FirstOrDefaultAsync(x =>
                x.Id == id);
    }


    // ============================================================
    // GET BY ID - ORGANIZATION SCOPED
    // ============================================================

    public async Task<Organization?> GetByIdAsync(
        Guid id,
        Guid organizationId)
    {
        return await _context.Organizations
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.Id == organizationId);
    }


    // ============================================================
    // GET ALL - SYSTEM ACCESS
    // ============================================================

    public async Task<IEnumerable<Organization>>
        GetAllAsync()
    {
        return await _context.Organizations
            .ToListAsync();
    }


    // ============================================================
    // EXISTS BY CODE
    // ============================================================

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeOrganizationId = null)
    {
        return await _context.Organizations
            .AnyAsync(x =>
                x.Code == code &&
                (!excludeOrganizationId.HasValue ||
                 x.Id != excludeOrganizationId.Value));
    }


    // ============================================================
    // UPDATE
    // ============================================================

    public Task UpdateAsync(
        Organization organization)
    {
        _context.Organizations.Update(
            organization);

        return Task.CompletedTask;
    }


    // ============================================================
    // SAVE
    // ============================================================

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}