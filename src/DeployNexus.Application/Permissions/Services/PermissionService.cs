using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.Permissions.DTOs;
using DeployNexus.Application.Permissions.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Permissions.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;


    public PermissionService(
        IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }


    public async Task<PermissionDto> CreateAsync(
        CreatePermissionRequest request)
    {
        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            IsActive = true
        };


        await _permissionRepository.AddAsync(permission);

        await _permissionRepository.SaveChangesAsync();


        return MapToDto(permission);
    }



    public async Task<PermissionDto?> GetByIdAsync(Guid id)
    {
        var permission =
            await _permissionRepository.GetByIdAsync(id);


        if (permission == null)
        {
            return null;
        }


        return MapToDto(permission);
    }



    public async Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        var permissions =
            await _permissionRepository.GetAllAsync();


        return permissions.Select(MapToDto);
    }



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


        await _permissionRepository.UpdateAsync(permission);

        await _permissionRepository.SaveChangesAsync();


        return MapToDto(permission);
    }



    public async Task<Result> DeactivateAsync(Guid id)
    {
        var permission =
            await _permissionRepository.GetByIdAsync(id);


        if (permission == null)
        {
            return Result.Failure("Permission not found");
        }


        permission.IsActive = false;


        await _permissionRepository.UpdateAsync(permission);

        await _permissionRepository.SaveChangesAsync();


        return Result.Ok();
    }



    private static PermissionDto MapToDto(
     Permission permission)
    {
        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Code = permission.Code,
            IsActive = permission.IsActive
        };
    }
}