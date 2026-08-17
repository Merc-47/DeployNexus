using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;
using DeployNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Infrastructure.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly DeployNexusDbContext _context;

    public ModuleRepository(
        DeployNexusDbContext context)
    {
        _context = context;
    }

    public async Task<Module> AddAsync(Module module)
    {
        await _context.Modules.AddAsync(module);

        return module;
    }

    public async Task<Module?> GetByIdAsync(Guid id)
    {
        return await _context.Modules
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Module>> GetAllAsync()
    {
        return await _context.Modules
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _context.Modules
            .AnyAsync(x => x.Code == code);
    }

    public Task UpdateAsync(Module module)
    {
        _context.Modules.Update(module);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}