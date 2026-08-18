using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Exceptions;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.Roles.DTOs;
using DeployNexus.Application.Roles.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Roles.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public RoleService(
        IRoleRepository roleRepository,
        IOrganizationRepository organizationRepository)
    {
        _roleRepository = roleRepository;
        _organizationRepository = organizationRepository;
    }


    public async Task<RoleDto> CreateAsync(
        CreateRoleRequest request)
    {
        var organization =
            await _organizationRepository
                .GetByIdAsync(request.OrganizationId);

        if (organization == null)
        {
            throw new NotFoundException(
                "Organization not found");
        }

        if (!organization.IsActive)
        {
            throw new ValidationException(
                "Organization is inactive");
        }


        var roleExists =
            await _roleRepository
                .ExistsByNameAsync(
                    request.OrganizationId,
                    request.Name);

        if (roleExists)
        {
            throw new ConflictException(
                "A role with this name already exists in the organization");
        }


        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            OrganizationId = request.OrganizationId
        };


        await _roleRepository.AddAsync(role);

        await _roleRepository.SaveChangesAsync();

        return MapToDto(role);
    }


    public async Task<RoleDto?> GetByIdAsync(
        Guid id)
    {
        var role =
            await _roleRepository
                .GetByIdAsync(id);

        if (role == null)
        {
            return null;
        }

        return MapToDto(role);
    }


    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles =
            await _roleRepository
                .GetAllAsync();

        return roles.Select(MapToDto);
    }


    public async Task<RoleDto?> UpdateAsync(
        Guid id,
        UpdateRoleRequest request)
    {
        var role =
            await _roleRepository
                .GetByIdAsync(id);

        if (role == null)
        {
            return null;
        }


        role.Name = request.Name;
        role.Description = request.Description;


        await _roleRepository
            .UpdateAsync(role);

        await _roleRepository
            .SaveChangesAsync();

        return MapToDto(role);
    }


    public async Task<Result> DeactivateAsync(
        Guid id)
    {
        var role =
            await _roleRepository
                .GetByIdAsync(id);

        if (role == null)
        {
            return Result.Failure(
                "Role not found");
        }


        if (!role.IsActive)
        {
            return Result.Failure(
                "Role is already inactive");
        }


        role.IsActive = false;


        await _roleRepository
            .UpdateAsync(role);

        await _roleRepository
            .SaveChangesAsync();

        return Result.Ok();
    }


    private static RoleDto MapToDto(
        Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            OrganizationId = role.OrganizationId
        };
    }
}