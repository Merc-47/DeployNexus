using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.Modules.DTOs;
using DeployNexus.Application.Modules.Interfaces;
using DeployNexus.Domain.Entities;
using DeployNexus.Domain.Enums;

namespace DeployNexus.Application.Modules.Services;

public class ModuleService : IModuleService
{
    private readonly IModuleRepository _moduleRepository;

    public ModuleService(
        IModuleRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }


    public async Task<ModuleDto> CreateAsync(
        CreateModuleRequest request)
    {
        var moduleExists =
            await _moduleRepository.ExistsByCodeAsync(
                request.Code);

        if (moduleExists)
        {
            throw new Exception(
                "A module with this code already exists");
        }


        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Status = ModuleStatus.Available
        };


        await _moduleRepository.AddAsync(module);

        await _moduleRepository.SaveChangesAsync();


        return MapToDto(module);
    }


    public async Task<ModuleDto?> GetByIdAsync(
        Guid id)
    {
        var module =
            await _moduleRepository.GetByIdAsync(id);


        if (module == null)
        {
            return null;
        }


        return MapToDto(module);
    }


    public async Task<IEnumerable<ModuleDto>> GetAllAsync()
    {
        var modules =
            await _moduleRepository.GetAllAsync();


        return modules.Select(MapToDto);
    }


    public async Task<ModuleDto?> UpdateAsync(
        Guid id,
        UpdateModuleRequest request)
    {
        var module =
            await _moduleRepository.GetByIdAsync(id);


        if (module == null)
        {
            return null;
        }


        module.Name = request.Name;
        module.Code = request.Code;
        module.Description = request.Description;


        await _moduleRepository.UpdateAsync(module);

        await _moduleRepository.SaveChangesAsync();


        return MapToDto(module);
    }


    public async Task<Result> DisableAsync(
        Guid id)
    {
        var module =
            await _moduleRepository.GetByIdAsync(id);


        if (module == null)
        {
            return Result.Failure(
                "Module not found");
        }


        module.Status = ModuleStatus.Disabled;


        await _moduleRepository.UpdateAsync(module);

        await _moduleRepository.SaveChangesAsync();


        return Result.Ok();
    }


    private static ModuleDto MapToDto(
        Module module)
    {
        return new ModuleDto
        {
            Id = module.Id,
            Name = module.Name,
            Code = module.Code,
            Description = module.Description,
            Status = module.Status
        };
    }
}