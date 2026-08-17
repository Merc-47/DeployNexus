using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.Permissions.DTOs;
using DeployNexus.Application.Permissions.Interfaces;
using DeployNexus.Domain.Entities;
using DeployNexus.Domain.Enums;

namespace DeployNexus.Application.Permissions.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IModuleRepository _moduleRepository;


    public PermissionService(
        IPermissionRepository permissionRepository,
        IModuleRepository moduleRepository)
    {
        _permissionRepository = permissionRepository;
        _moduleRepository = moduleRepository;
    }


    // ============================================================
    // CREATE
    // ============================================================

    public async Task<PermissionDto> CreateAsync(
        CreatePermissionRequest request)
    {
        // Make sure the module exists.
        var module =
            await _moduleRepository.GetByIdAsync(
                request.ModuleId);

        if (module == null)
        {
            throw new Exception(
                "Module not found");
        }


        // Don't allow permissions to be created
        // under a disabled module.
        if (module.Status == ModuleStatus.Disabled)
        {
            throw new Exception(
                "Cannot create permission for a disabled module");
        }


        var permission = new Permission
        {
            Id = Guid.NewGuid(),

            Name = request.Name,

            Code = request.Code,

            ModuleId = request.ModuleId,

            IsActive = true
        };


        await _permissionRepository.AddAsync(
            permission);

        await _permissionRepository.SaveChangesAsync();


        return MapToDto(permission);
    }


    // ============================================================
    // GET BY ID
    // ============================================================

    public async Task<PermissionDto?> GetByIdAsync(
        Guid id)
    {
        var permission =
            await _permissionRepository.GetByIdAsync(id);


        if (permission == null)
        {
            return null;
        }


        return MapToDto(permission);
    }


    // ============================================================
    // GET ALL
    // ============================================================

    public async Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        var permissions =
            await _permissionRepository.GetAllAsync();


        return permissions.Select(MapToDto);
    }


    // ============================================================
    // UPDATE
    // ============================================================

    public async Task<PermissionDto?> UpdateAsync(
        Guid id,
        UpdatePermissionRequest request)
    {
        var permission =
            await _permissionRepository.GetByIdAsync(id);


        if (permission == null)
        {
            return null;
        }


        permission.Name = request.Name;

        permission.Code = request.Code;


        await _permissionRepository.UpdateAsync(
            permission);

        await _permissionRepository.SaveChangesAsync();


        return MapToDto(permission);
    }


    // ============================================================
    // DEACTIVATE
    // ============================================================

    public async Task<Result> DeactivateAsync(
        Guid id)
    {
        var permission =
            await _permissionRepository.GetByIdAsync(id);


        if (permission == null)
        {
            return Result.Failure(
                "Permission not found");
        }


        permission.IsActive = false;


        await _permissionRepository.UpdateAsync(
            permission);

        await _permissionRepository.SaveChangesAsync();


        return Result.Ok();
    }


    // ============================================================
    // CHECK USER PERMISSION
    // ============================================================

    public async Task<bool> HasPermissionAsync(
        Guid userId,
        string permissionCode)
    {
        return await _permissionRepository
            .UserHasPermissionAsync(
                userId,
                permissionCode);
    }


    // ============================================================
    // MAPPING
    // ============================================================

    private static PermissionDto MapToDto(
        Permission permission)
    {
        return new PermissionDto
        {
            Id = permission.Id,

            Name = permission.Name,

            Code = permission.Code,

            ModuleId = permission.ModuleId,

            IsActive = permission.IsActive
        };
    }
}