using DeployNexus.Application.Organizations.DTOs;
using DeployNexus.Application.Organizations.Services;

namespace DeployNexus.Tests.Organizations;

public class OrganizationServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateOrganization()
    {
        var service = new OrganizationService();

        var result = await service.CreateAsync(new CreateOrganizationRequest
        {
            Name = "Acme Corporation",
            Code = "ACME"
        });

        Assert.NotNull(result);
        Assert.Equal("Acme Corporation", result.Name);
        Assert.Equal("ACME", result.Code);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrganization()
    {
        var service = new OrganizationService();

        var created = await service.CreateAsync(new CreateOrganizationRequest
        {
            Name = "Acme",
            Code = "ACME"
        });

        var result = await service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrganizations()
    {
        var service = new OrganizationService();

        await service.CreateAsync(new CreateOrganizationRequest
        {
            Name = "Org 1",
            Code = "ORG1"
        });

        await service.CreateAsync(new CreateOrganizationRequest
        {
            Name = "Org 2",
            Code = "ORG2"
        });

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrganization()
    {
        var service = new OrganizationService();

        var created = await service.CreateAsync(new CreateOrganizationRequest
        {
            Name = "Old Name",
            Code = "OLD"
        });

        var result = await service.UpdateAsync(created.Id,
            new UpdateOrganizationRequest
            {
                Name = "New Name",
                Code = "NEW"
            });

        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("NEW", result.Code);
    }

    [Fact]
    public async Task DeactivateAsync_ShouldDeactivateOrganization()
    {
        var service = new OrganizationService();

        var created = await service.CreateAsync(new CreateOrganizationRequest
        {
            Name = "Acme",
            Code = "ACME"
        });

        var result = await service.DeactivateAsync(created.Id);

        var organization = await service.GetByIdAsync(created.Id);

        Assert.True(result.Success);
        Assert.NotNull(organization);
        Assert.False(organization.IsActive);
    }
}