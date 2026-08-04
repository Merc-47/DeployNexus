using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;
using DeployNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly DeployNexusDbContext _context;

    public RoleRepository(DeployNexusDbContext context)
    {
        _context = context;
    }


    public async Task<Role> AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);

        return role;
    }


    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .ToListAsync();
    }


    public Task UpdateAsync(Role role)
    {
        _context.Roles.Update(role);

        return Task.CompletedTask;
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}