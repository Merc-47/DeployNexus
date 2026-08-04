using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;
using DeployNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly DeployNexusDbContext _context;


    public PermissionRepository(
        DeployNexusDbContext context)
    {
        _context = context;
    }


    public async Task<Permission> AddAsync(Permission permission)
    {
        await _context.Permissions.AddAsync(permission);

        return permission;
    }


    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _context.Permissions
            .ToListAsync();
    }


    public Task UpdateAsync(Permission permission)
    {
        _context.Permissions.Update(permission);

        return Task.CompletedTask;
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}