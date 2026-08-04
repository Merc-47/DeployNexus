using DeployNexus.Application.Common;
using DeployNexus.Application.Permissions.DTOs;

namespace DeployNexus.Application.Permissions.Interfaces;

public interface IPermissionService
{
    Task<PermissionDto> CreateAsync(CreatePermissionRequest request);

    Task<PermissionDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<PermissionDto>> GetAllAsync();

    Task<PermissionDto?> UpdateAsync(
        Guid id,
        UpdatePermissionRequest request);

    Task<Result> DeactivateAsync(Guid id);
}