using DeployNexus.Application.Common;
using DeployNexus.Application.Organizations.DTOs;
using DeployNexus.Application.Organizations.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Organizations.Services;

public class OrganizationService : IOrganizationService
{
    private readonly List<Organization> _organizations = new();

    public Task<OrganizationDto> CreateAsync(CreateOrganizationRequest request)
    {
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            IsActive = true
        };

        _organizations.Add(organization);

        return Task.FromResult(MapToDto(organization));
    }

    public Task<OrganizationDto?> GetByIdAsync(Guid id)
    {
        var organization = _organizations.FirstOrDefault(x => x.Id == id);

        if (organization == null)
            return Task.FromResult<OrganizationDto?>(null);

        return Task.FromResult<OrganizationDto?>(MapToDto(organization));
    }

    public Task<IEnumerable<OrganizationDto>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<OrganizationDto>>(
            _organizations.Select(MapToDto).ToList());
    }

    public Task<OrganizationDto?> UpdateAsync(Guid id, UpdateOrganizationRequest request)
    {
        var organization = _organizations.FirstOrDefault(x => x.Id == id);

        if (organization == null)
            return Task.FromResult<OrganizationDto?>(null);

        organization.Name = request.Name;
        organization.Code = request.Code;

        return Task.FromResult<OrganizationDto?>(MapToDto(organization));
    }

    public Task<Result> DeactivateAsync(Guid id)
    {
        var organization = _organizations.FirstOrDefault(x => x.Id == id);

        if (organization == null)
            return Task.FromResult(Result.Failure("Organization not found"));

        organization.IsActive = false;

        return Task.FromResult(Result.Ok());
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
