using DeployNexus.Application.Common;
using DeployNexus.Application.Roles.DTOs;

namespace DeployNexus.Application.Roles.Interfaces;

public interface IRoleService
{
    Task<RoleDto> CreateAsync(CreateRoleRequest request);

    Task<RoleDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<RoleDto>> GetAllAsync();

    Task<RoleDto?> UpdateAsync(Guid id, UpdateRoleRequest request);

    Task<Result> DeactivateAsync(Guid id);
}