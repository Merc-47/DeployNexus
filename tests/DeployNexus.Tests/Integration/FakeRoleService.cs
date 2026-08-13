using DeployNexus.Application.Common;
using DeployNexus.Application.Roles.DTOs;
using DeployNexus.Application.Roles.Interfaces;

namespace DeployNexus.Tests.Integration;

public class FakeRoleService : IRoleService
{
    private readonly List<RoleDto> _roles = new();

    public Task<RoleDto> CreateAsync(
        CreateRoleRequest request)
    {
        var role = new RoleDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            OrganizationId = request.OrganizationId
        };

        _roles.Add(role);

        return Task.FromResult(role);
    }


    public Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = _roles
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult(role);
    }


    public Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        return Task.FromResult(
            _roles.AsEnumerable());
    }


    public Task<RoleDto?> UpdateAsync(
        Guid id,
        UpdateRoleRequest request)
    {
        var role = _roles
            .FirstOrDefault(x => x.Id == id);

        if (role == null)
        {
            return Task.FromResult<RoleDto?>(null);
        }

        role.Name = request.Name;
        role.Description = request.Description;

        return Task.FromResult<RoleDto?>(role);
    }


    public Task<Result> DeactivateAsync(Guid id)
    {
        var role = _roles
            .FirstOrDefault(x => x.Id == id);

        if (role == null)
        {
            return Task.FromResult(
                Result.Failure("Role not found."));
        }

        role.IsActive = false;

        return Task.FromResult(
            Result.Ok());
    }
}