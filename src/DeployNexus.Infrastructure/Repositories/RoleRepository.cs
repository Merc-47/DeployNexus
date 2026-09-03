using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;
using DeployNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly DeployNexusDbContext _context;

    public RoleRepository(
        DeployNexusDbContext context)
    {
        _context = context;
    }


    // ============================================================
    // CREATE
    // ============================================================

    public async Task<Role> AddAsync(
        Role role)
    {
        await _context.Roles.AddAsync(role);

        return role;
    }


    // ============================================================
    // GET BY ID - SYSTEM ACCESS
    // ============================================================

    public async Task<Role?> GetByIdAsync(
        Guid id)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(x =>
                x.Id == id);
    }


    // ============================================================
    // GET BY ID - ORGANIZATION SCOPED
    // ============================================================

    public async Task<Role?> GetByIdAsync(
        Guid id,
        Guid organizationId)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.OrganizationId == organizationId);
    }


    // ============================================================
    // GET ALL - SYSTEM ACCESS
    // ============================================================

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .ToListAsync();
    }


    // ============================================================
    // GET ALL - ORGANIZATION SCOPED
    // ============================================================

    public async Task<IEnumerable<Role>> GetAllAsync(
        Guid organizationId)
    {
        return await _context.Roles
            .Where(x =>
                x.OrganizationId == organizationId)
            .ToListAsync();
    }


    // ============================================================
    // CHECK ROLE NAME
    // ============================================================

    public async Task<bool> ExistsByNameAsync(
        Guid organizationId,
        string name)
    {
        return await _context.Roles
            .AnyAsync(x =>
                x.OrganizationId == organizationId &&
                x.Name == name);
    }


    // ============================================================
    // UPDATE
    // ============================================================

    public Task UpdateAsync(
        Role role)
    {
        _context.Roles.Update(role);

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