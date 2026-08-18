using DeployNexus.Application.Common;
using DeployNexus.Application.Modules.DTOs;

namespace DeployNexus.Application.Modules.Interfaces;

public interface IModuleService
{
    Task<ModuleDto> CreateAsync(
        CreateModuleRequest request);

    Task<ModuleDto?> GetByIdAsync(
        Guid id);

    Task<IEnumerable<ModuleDto>> GetAllAsync();

    Task<ModuleDto?> UpdateAsync(
        Guid id,
        UpdateModuleRequest request);

    Task<Result> DisableAsync(
        Guid id);
}