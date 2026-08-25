using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Exceptions;
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


    // ============================================================
    // CREATE
    // ============================================================

    public async Task<ModuleDto> CreateAsync(
        CreateModuleRequest request)
    {
        if (request == null)
        {
            throw new ValidationException(
                "Module request is required");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException(
                "Module name is required");
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new ValidationException(
                "Module code is required");
        }


        var moduleExists =
            await _moduleRepository.ExistsByCodeAsync(
                request.Code);

        if (moduleExists)
        {
            throw new ConflictException(
                "A module with this code already exists");
        }


        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Description = request.Description,
            Status = ModuleStatus.Available
        };


        await _moduleRepository.AddAsync(module);

        await _moduleRepository.SaveChangesAsync();


        return MapToDto(module);
    }


    // ============================================================
    // GET BY ID
    // ============================================================

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


    // ============================================================
    // GET ALL
    // ============================================================

    public async Task<IEnumerable<ModuleDto>> GetAllAsync()
    {
        var modules =
            await _moduleRepository.GetAllAsync();


        return modules.Select(MapToDto);
    }


    // ============================================================
    // UPDATE
    // ============================================================

    public async Task<ModuleDto?> UpdateAsync(
        Guid id,
        UpdateModuleRequest request)
    {
        if (request == null)
        {
            throw new ValidationException(
                "Module request is required");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException(
                "Module name is required");
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new ValidationException(
                "Module code is required");
        }


        var module =
            await _moduleRepository.GetByIdAsync(id);


        if (module == null)
        {
            return null;
        }


        var normalizedCode =
            request.Code.Trim().ToUpperInvariant();


        // Only check duplicate code if the code
        // is actually being changed.
        if (!string.Equals(
                module.Code,
                normalizedCode,
                StringComparison.OrdinalIgnoreCase))
        {
            var moduleExists =
                await _moduleRepository.ExistsByCodeAsync(
                    normalizedCode);

            if (moduleExists)
            {
                throw new ConflictException(
                    "A module with this code already exists");
            }
        }


        module.Name = request.Name.Trim();
        module.Code = normalizedCode;
        module.Description = request.Description;


        await _moduleRepository.UpdateAsync(module);

        await _moduleRepository.SaveChangesAsync();


        return MapToDto(module);
    }


    // ============================================================
    // DISABLE
    // ============================================================

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


        if (module.Status == ModuleStatus.Disabled)
        {
            return Result.Failure(
                "Module is already disabled");
        }


        module.Status = ModuleStatus.Disabled;


        await _moduleRepository.UpdateAsync(module);

        await _moduleRepository.SaveChangesAsync();


        return Result.Ok();
    }


    // ============================================================
    // MAPPING
    // ============================================================

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