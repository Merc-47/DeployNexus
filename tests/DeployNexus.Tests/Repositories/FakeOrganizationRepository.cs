using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Tests.Organizations;

public class FakeOrganizationRepository : IOrganizationRepository
{
    private readonly List<Organization> _organizations = new();


    public Task<Organization> AddAsync(Organization organization)
    {
        _organizations.Add(organization);

        return Task.FromResult(organization);
    }


    public Task<Organization?> GetByIdAsync(Guid id)
    {
        var organization = _organizations
            .FirstOrDefault(x => x.Id == id);

        return Task.FromResult(organization);
    }


    public Task<IEnumerable<Organization>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Organization>>(
            _organizations);
    }


    public Task UpdateAsync(Organization organization)
    {
        var existing = _organizations
            .FirstOrDefault(x => x.Id == organization.Id);

        if (existing != null)
        {
            existing.Name = organization.Name;
            existing.Code = organization.Code;
            existing.IsActive = organization.IsActive;
        }

        return Task.CompletedTask;
    }


    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}