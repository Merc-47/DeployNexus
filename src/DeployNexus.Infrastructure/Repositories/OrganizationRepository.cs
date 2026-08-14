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

    public async Task<Organization> AddAsync(
        Organization organization)
    {
        await _context.Organizations.AddAsync(organization);

        return organization;
    }

    public async Task<Organization?> GetByIdAsync(
        Guid id)
    {
        return await _context.Organizations
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Organization>> GetAllAsync()
    {
        return await _context.Organizations
            .ToListAsync();
    }

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

    public Task UpdateAsync(
        Organization organization)
    {
        _context.Organizations.Update(organization);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}