using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Exceptions;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.Roles.DTOs;
using DeployNexus.Application.Roles.Interfaces;
using DeployNexus.Domain.Entities;
using DeployNexus.Domain.Enums;

namespace DeployNexus.Application.Roles.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ICurrentUserService _currentUserService;

    public RoleService(
        IRoleRepository roleRepository,
        IOrganizationRepository organizationRepository,
        ICurrentUserService currentUserService)
    {
        _roleRepository = roleRepository;
        _organizationRepository = organizationRepository;
        _currentUserService = currentUserService;
    }


    // ============================================================
    // CREATE ROLE
    // ============================================================

    public async Task<RoleDto> CreateAsync(
        CreateRoleRequest request)
    {
        // --------------------------------------------------------
        // Organization access
        // --------------------------------------------------------

        EnsureOrganizationAccess(
            request.OrganizationId);


        // --------------------------------------------------------
        // Validate organization
        // --------------------------------------------------------

        var organization =
            await _organizationRepository
                .GetByIdAsync(
                    request.OrganizationId);

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


        // --------------------------------------------------------
        // Role name uniqueness
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // Create role
        // --------------------------------------------------------

        var role = new Role
        {
            Id = Guid.NewGuid(),

            Name = request.Name,

            Description = request.Description,

            IsActive = true,

            OrganizationId =
                request.OrganizationId,

            RoleType = RoleType.Organization
        };


        await _roleRepository
            .AddAsync(role);

        await _roleRepository
            .SaveChangesAsync();


        return MapToDto(role);
    }


    // ============================================================
    // GET ROLE BY ID
    // ============================================================

    public async Task<RoleDto?> GetByIdAsync(
        Guid id)
    {
        Role? role;


        // --------------------------------------------------------
        // System user
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            role =
                await _roleRepository
                    .GetByIdAsync(id);
        }


        // --------------------------------------------------------
        // Organization user
        // --------------------------------------------------------

        else
        {
            var organizationId =
                GetCurrentOrganizationId();

            role =
                await _roleRepository
                    .GetByIdAsync(
                        id,
                        organizationId);
        }


        if (role == null)
        {
            return null;
        }


        return MapToDto(role);
    }


    // ============================================================
    // GET ALL ROLES
    // ============================================================

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        IEnumerable<Role> roles;


        // --------------------------------------------------------
        // System user
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            roles =
                await _roleRepository
                    .GetAllAsync();
        }


        // --------------------------------------------------------
        // Organization user
        // --------------------------------------------------------

        else
        {
            var organizationId =
                GetCurrentOrganizationId();

            roles =
                await _roleRepository
                    .GetAllAsync(
                        organizationId);
        }


        return roles.Select(MapToDto);
    }


    // ============================================================
    // UPDATE ROLE
    // ============================================================

    public async Task<RoleDto?> UpdateAsync(
        Guid id,
        UpdateRoleRequest request)
    {
        Role? role;


        // --------------------------------------------------------
        // Find role within user's accessible scope
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            role =
                await _roleRepository
                    .GetByIdAsync(id);
        }
        else
        {
            var organizationId =
                GetCurrentOrganizationId();

            role =
                await _roleRepository
                    .GetByIdAsync(
                        id,
                        organizationId);
        }


        if (role == null)
        {
            return null;
        }


        // --------------------------------------------------------
        // Update role
        // --------------------------------------------------------

        role.Name =
            request.Name;

        role.Description =
            request.Description;


        await _roleRepository
            .UpdateAsync(role);

        await _roleRepository
            .SaveChangesAsync();


        return MapToDto(role);
    }


    // ============================================================
    // DEACTIVATE ROLE
    // ============================================================

    public async Task<Result> DeactivateAsync(
        Guid id)
    {
        Role? role;


        // --------------------------------------------------------
        // Find role within user's accessible scope
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            role =
                await _roleRepository
                    .GetByIdAsync(id);
        }
        else
        {
            var organizationId =
                GetCurrentOrganizationId();

            role =
                await _roleRepository
                    .GetByIdAsync(
                        id,
                        organizationId);
        }


        if (role == null)
        {
            return Result.Failure(
                "Role not found");
        }


        // --------------------------------------------------------
        // Already inactive
        // --------------------------------------------------------

        if (!role.IsActive)
        {
            return Result.Failure(
                "Role is already inactive");
        }


        // --------------------------------------------------------
        // Deactivate
        // --------------------------------------------------------

        role.IsActive = false;


        await _roleRepository
            .UpdateAsync(role);

        await _roleRepository
            .SaveChangesAsync();


        return Result.Ok();
    }


    // ============================================================
    // ORGANIZATION ACCESS
    // ============================================================

    private void EnsureOrganizationAccess(
        Guid organizationId)
    {
        // --------------------------------------------------------
        // System users can access every organization.
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            return;
        }


        // --------------------------------------------------------
        // User must be authenticated.
        // --------------------------------------------------------

        if (!_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated");
        }


        // --------------------------------------------------------
        // Organization must exist in JWT.
        // --------------------------------------------------------

        if (!_currentUserService.OrganizationId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User organization could not be determined");
        }


        // --------------------------------------------------------
        // Organization user can only access their own
        // organization.
        // --------------------------------------------------------

        if (_currentUserService.OrganizationId.Value
            != organizationId)
        {
            throw new UnauthorizedAccessException(
                "You cannot access resources outside your organization");
        }
    }


    // ============================================================
    // CURRENT ORGANIZATION
    // ============================================================

    private Guid GetCurrentOrganizationId()
    {
        if (!_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated");
        }


        if (!_currentUserService.OrganizationId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User organization could not be determined");
        }


        return _currentUserService.OrganizationId.Value;
    }


    // ============================================================
    // MAPPING
    // ============================================================

    private static RoleDto MapToDto(
        Role role)
    {
        return new RoleDto
        {
            Id = role.Id,

            Name = role.Name,

            Description = role.Description,

            IsActive = role.IsActive,

            OrganizationId =
                role.OrganizationId
        };
    }
}