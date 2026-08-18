using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Tests.Repositories;

public class FakeModuleRepository : IModuleRepository
{
    private readonly List<Module> _modules = new();


    public Task<Module> AddAsync(Module module)
    {
        _modules.Add(module);

        return Task.FromResult(module);
    }


    public Task<Module?> GetByIdAsync(Guid id)
    {
        var module = _modules
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult(module);
    }


    public Task<IEnumerable<Module>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Module>>(
            _modules);
    }


    public Task<bool> ExistsByCodeAsync(string code)
    {
        var exists = _modules.Any(
            x => x.Code == code);

        return Task.FromResult(exists);
    }


    public Task UpdateAsync(Module module)
    {
        var existing = _modules
            .FirstOrDefault(x => x.Id == module.Id);

        if (existing != null)
        {
            existing.Name = module.Name;
            existing.Code = module.Code;
            existing.Description = module.Description;
            existing.Status = module.Status;
        }

        return Task.CompletedTask;
    }


    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}