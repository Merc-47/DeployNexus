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

    public async Task<OrganizationDto> CreateAsync(CreateOrganizationRequest request)
    {
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            IsActive = true
        };

        await _organizationRepository.AddAsync(organization);

        await _organizationRepository.SaveChangesAsync();

        return MapToDto(organization);
    }

    public async Task<OrganizationDto?> GetByIdAsync(Guid id)
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
        var organization = await _organizationRepository.GetByIdAsync(id);

        if (organization == null)
            return null;

        organization.Name = request.Name;
        organization.Code = request.Code;

        await _organizationRepository.UpdateAsync(organization);

        await _organizationRepository.SaveChangesAsync();

        return MapToDto(organization);
    }

    public async Task<Result> DeactivateAsync(Guid id)
    {
        var organization = await _organizationRepository.GetByIdAsync(id);

        if (organization == null)
            return Result.Failure("Organization not found");

        organization.IsActive = false;

        await _organizationRepository.UpdateAsync(organization);

        await _organizationRepository.SaveChangesAsync();

        return Result.Ok();
    }

    private static OrganizationDto MapToDto(Organization organization)
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
