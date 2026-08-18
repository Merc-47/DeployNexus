using DeployNexus.Application.Common;
using DeployNexus.Application.Modules.DTOs;
using DeployNexus.Application.Modules.Interfaces;
using DeployNexus.Domain.Enums;

namespace DeployNexus.Tests.Integration;

public class FakeModuleService : IModuleService
{
    private readonly Dictionary<Guid, ModuleDto> _modules = new();


    public Task<ModuleDto> CreateAsync(
        CreateModuleRequest request)
    {
        var module = new ModuleDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Status = ModuleStatus.Available
        };

        _modules[module.Id] = module;

        return Task.FromResult(module);
    }


    public Task<ModuleDto?> GetByIdAsync(
        Guid id)
    {
        _modules.TryGetValue(
            id,
            out var module);

        return Task.FromResult(module);
    }


    public Task<IEnumerable<ModuleDto>> GetAllAsync()
    {
        return Task.FromResult<
            IEnumerable<ModuleDto>>(
                _modules.Values);
    }


    public Task<ModuleDto?> UpdateAsync(
        Guid id,
        UpdateModuleRequest request)
    {
        if (!_modules.TryGetValue(
                id,
                out var module))
        {
            return Task.FromResult<ModuleDto?>(
                null);
        }

        module.Name = request.Name;
        module.Code = request.Code;
        module.Description = request.Description;

        return Task.FromResult<ModuleDto?>(
            module);
    }


    public Task<Result> DisableAsync(
        Guid id)
    {
        if (!_modules.TryGetValue(
                id,
                out var module))
        {
            return Task.FromResult(
                Result.Failure(
                    "Module not found"));
        }

        module.Status = ModuleStatus.Disabled;

        return Task.FromResult(
            Result.Ok());
    }
}