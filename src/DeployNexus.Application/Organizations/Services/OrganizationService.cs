using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.Organizations.DTOs;
using DeployNexus.Application.Organizations.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Organizations.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationService(
        IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<OrganizationDto> CreateAsync(
        CreateOrganizationRequest request)
    {
        ValidateName(request.Name);
        ValidateCode(request.Code);

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var codeExists =
            await _organizationRepository.ExistsByCodeAsync(
                normalizedCode);

        if (codeExists)
        {
            throw new InvalidOperationException(
                "Organization code already exists");
        }

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = normalizedCode,
            IsActive = true
        };

        await _organizationRepository.AddAsync(
            organization);

        await _organizationRepository.SaveChangesAsync();

        return MapToDto(organization);
    }

    public async Task<OrganizationDto?> GetByIdAsync(
        Guid id)
    {
        var organization =
            await _organizationRepository.GetByIdAsync(id);

        if (organization == null)
            return null;

        return MapToDto(organization);
    }

    public async Task<IEnumerable<OrganizationDto>> GetAllAsync()
    {
        var organizations =
            await _organizationRepository.GetAllAsync();

        return organizations.Select(MapToDto);
    }

    public async Task<OrganizationDto?> UpdateAsync(
        Guid id,
        UpdateOrganizationRequest request)
    {
        ValidateName(request.Name);
        ValidateCode(request.Code);

        var organization =
            await _organizationRepository.GetByIdAsync(id);

        if (organization == null)
            return null;

        var normalizedCode =
            request.Code.Trim().ToUpperInvariant();

        var codeExists =
            await _organizationRepository.ExistsByCodeAsync(
                normalizedCode,
                id);

        if (codeExists)
        {
            throw new InvalidOperationException(
                "Organization code already exists");
        }

        organization.Name = request.Name.Trim();
        organization.Code = normalizedCode;

        await _organizationRepository.UpdateAsync(
            organization);

        await _organizationRepository.SaveChangesAsync();

        return MapToDto(organization);
    }

    public async Task<Result> DeactivateAsync(
        Guid id)
    {
        var organization =
            await _organizationRepository.GetByIdAsync(id);

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

        await _organizationRepository.UpdateAsync(
            organization);

        await _organizationRepository.SaveChangesAsync();

        return Result.Ok();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Organization name is required");
        }

        if (name.Trim().Length < 2)
        {
            throw new ArgumentException(
                "Organization name must contain at least 2 characters");
        }
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Organization code is required");
        }

        if (code.Trim().Length < 2)
        {
            throw new ArgumentException(
                "Organization code must contain at least 2 characters");
        }
    }

    private static OrganizationDto MapToDto(
        Organization organization)
    {
        return new OrganizationDto
        {
            Id = organization.Id,
            Name = organization.Name,
            Code = organization.Code,
            IsActive = organization.IsActive
        };
    }
}