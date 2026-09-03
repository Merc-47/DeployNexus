using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Exceptions;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.Organizations.DTOs;
using DeployNexus.Application.Organizations.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Organizations.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ICurrentUserService _currentUserService;


    public OrganizationService(
        IOrganizationRepository organizationRepository,
        ICurrentUserService currentUserService)
    {
        _organizationRepository =
            organizationRepository;

        _currentUserService =
            currentUserService;
    }


    // ============================================================
    // CREATE ORGANIZATION
    // ============================================================

    public async Task<OrganizationDto> CreateAsync(
        CreateOrganizationRequest request)
    {
        // --------------------------------------------------------
        // Only system users can create organizations.
        // --------------------------------------------------------

        EnsureSystemUser();


        ValidateName(request.Name);
        ValidateCode(request.Code);


        var normalizedCode =
            request.Code
                .Trim()
                .ToUpperInvariant();


        var codeExists =
            await _organizationRepository
                .ExistsByCodeAsync(
                    normalizedCode);


        if (codeExists)
        {
            throw new ConflictException(
                "Organization code already exists");
        }


        var organization = new Organization
        {
            Id = Guid.NewGuid(),

            Name =
                request.Name.Trim(),

            Code =
                normalizedCode,

            IsActive = true
        };


        await _organizationRepository
            .AddAsync(organization);

        await _organizationRepository
            .SaveChangesAsync();


        return MapToDto(
            organization);
    }


    // ============================================================
    // GET ORGANIZATION BY ID
    // ============================================================

    public async Task<OrganizationDto?> GetByIdAsync(
        Guid id)
    {
        Organization? organization;


        // --------------------------------------------------------
        // System user
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            organization =
                await _organizationRepository
                    .GetByIdAsync(id);
        }


        // --------------------------------------------------------
        // Organization user
        // --------------------------------------------------------

        else
        {
            var organizationId =
                GetCurrentOrganizationId();


            organization =
                await _organizationRepository
                    .GetByIdAsync(
                        id,
                        organizationId);
        }


        if (organization == null)
        {
            return null;
        }


        return MapToDto(
            organization);
    }


    // ============================================================
    // GET ALL ORGANIZATIONS
    // ============================================================

    public async Task<IEnumerable<OrganizationDto>>
        GetAllAsync()
    {
        // --------------------------------------------------------
        // System users can see every organization.
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            var organizations =
                await _organizationRepository
                    .GetAllAsync();

            return organizations
                .Select(MapToDto);
        }


        // --------------------------------------------------------
        // Organization users can ONLY see their own
        // organization.
        // --------------------------------------------------------

        var organizationId =
            GetCurrentOrganizationId();


        var organization =
            await _organizationRepository
                .GetByIdAsync(
                    organizationId,
                    organizationId);


        if (organization == null)
        {
            return Enumerable.Empty<OrganizationDto>();
        }


        return new[]
        {
            MapToDto(organization)
        };
    }


    // ============================================================
    // UPDATE ORGANIZATION
    // ============================================================

    public async Task<OrganizationDto?> UpdateAsync(
        Guid id,
        UpdateOrganizationRequest request)
    {
        // --------------------------------------------------------
        // Only system users can update organizations.
        // --------------------------------------------------------

        EnsureSystemUser();


        ValidateName(request.Name);
        ValidateCode(request.Code);


        var organization =
            await _organizationRepository
                .GetByIdAsync(id);


        if (organization == null)
        {
            return null;
        }


        var normalizedCode =
            request.Code
                .Trim()
                .ToUpperInvariant();


        var codeExists =
            await _organizationRepository
                .ExistsByCodeAsync(
                    normalizedCode,
                    id);


        if (codeExists)
        {
            throw new ConflictException(
                "Organization code already exists");
        }


        organization.Name =
            request.Name.Trim();

        organization.Code =
            normalizedCode;


        await _organizationRepository
            .UpdateAsync(
                organization);

        await _organizationRepository
            .SaveChangesAsync();


        return MapToDto(
            organization);
    }


    // ============================================================
    // DEACTIVATE ORGANIZATION
    // ============================================================

    public async Task<Result> DeactivateAsync(
        Guid id)
    {
        // --------------------------------------------------------
        // Only system users can deactivate organizations.
        // --------------------------------------------------------

        EnsureSystemUser();


        var organization =
            await _organizationRepository
                .GetByIdAsync(id);


        if (organization == null)
        {
            return Result.Failure(
                "Organization not found");
        }


        if (!organization.IsActive)
        {
            return Result.Failure(
                "Organization is already inactive");
        }


        organization.IsActive = false;


        await _organizationRepository
            .UpdateAsync(
                organization);

        await _organizationRepository
            .SaveChangesAsync();


        return Result.Ok();
    }


    // ============================================================
    // SYSTEM USER CHECK
    // ============================================================

    private void EnsureSystemUser()
    {
        if (!_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated");
        }


        if (!_currentUserService.IsSystemUser)
        {
            throw new UnauthorizedAccessException(
                "Only system users can perform this operation");
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
    // VALIDATION
    // ============================================================

    private static void ValidateName(
        string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException(
                "Organization name is required");
        }


        if (name.Trim().Length < 2)
        {
            throw new ValidationException(
                "Organization name must contain at least 2 characters");
        }
    }


    private static void ValidateCode(
        string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ValidationException(
                "Organization code is required");
        }


        if (code.Trim().Length < 2)
        {
            throw new ValidationException(
                "Organization code must contain at least 2 characters");
        }
    }


    // ============================================================
    // MAPPING
    // ============================================================

    private static OrganizationDto MapToDto(
        Organization organization)
    {
        return new OrganizationDto
        {
            Id = organization.Id,

            Name =
                organization.Name,

            Code =
                organization.Code,

            IsActive =
                organization.IsActive
        };
    }
}