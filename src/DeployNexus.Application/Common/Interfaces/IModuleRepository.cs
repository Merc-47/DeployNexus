using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Common.Interfaces;

public interface IModuleRepository
{
    Task<Module> AddAsync(Module module);

    Task<Module?> GetByIdAsync(Guid id);

    Task<IEnumerable<Module>> GetAllAsync();

    Task<bool> ExistsByCodeAsync(string code);

    Task UpdateAsync(Module module);

    Task SaveChangesAsync();
}