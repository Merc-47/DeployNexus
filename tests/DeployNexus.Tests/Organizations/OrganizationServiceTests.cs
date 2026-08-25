using DeployNexus.Application.Common.Exceptions;
using DeployNexus.Application.Organizations.DTOs;
using DeployNexus.Application.Organizations.Services;
using DeployNexus.Tests.Repositories;

namespace DeployNexus.Tests.Organizations;

public class OrganizationServiceTests
{
    private OrganizationService CreateService()
    {
        var repository = new FakeOrganizationRepository();

        return new OrganizationService(repository);
    }


    [Fact]
    public async Task CreateAsync_ShouldCreateOrganization()
    {
        var service = CreateService();

        var result = await service.CreateAsync(
            new CreateOrganizationRequest
            {
                Name = "Acme Corporation",
                Code = "ACME"
            });

        Assert.NotNull(result);
        Assert.Equal(
            "Acme Corporation",
            result.Name);

        Assert.Equal(
            "ACME",
            result.Code);

        Assert.True(result.IsActive);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrganization()
    {
        var service = CreateService();

        var created =
            await service.CreateAsync(
                new CreateOrganizationRequest
                {
                    Name = "Acme",
                    Code = "ACME"
                });

        var result =
            await service.GetByIdAsync(created.Id);

        Assert.NotNull(result);

        Assert.Equal(
            created.Id,
            result.Id);
    }


    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrganizations()
    {
        var service = CreateService();

        await service.CreateAsync(
            new CreateOrganizationRequest
            {
                Name = "Org 1",
                Code = "ORG1"
            });

        await service.CreateAsync(
            new CreateOrganizationRequest
            {
                Name = "Org 2",
                Code = "ORG2"
            });

        var result =
            await service.GetAllAsync();

        Assert.Equal(
            2,
            result.Count());
    }


    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrganization()
    {
        var service = CreateService();

        var created =
            await service.CreateAsync(
                new CreateOrganizationRequest
                {
                    Name = "Old Name",
                    Code = "OLD"
                });

        var result =
            await service.UpdateAsync(
                created.Id,
                new UpdateOrganizationRequest
                {
                    Name = "New Name",
                    Code = "NEW"
                });

        Assert.NotNull(result);

        Assert.Equal(
            "New Name",
            result.Name);

        Assert.Equal(
            "NEW",
            result.Code);
    }


    [Fact]
    public async Task DeactivateAsync_ShouldDeactivateOrganization()
    {
        var service = CreateService();

        var created =
            await service.CreateAsync(
                new CreateOrganizationRequest
                {
                    Name = "Acme",
                    Code = "ACME"
                });

        var result =
            await service.DeactivateAsync(created.Id);

        var organization =
            await service.GetByIdAsync(created.Id);

        Assert.True(result.Success);

        Assert.NotNull(organization);

        Assert.False(
            organization.IsActive);
    }


    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ShouldThrow()
    {
        var service = CreateService();

        await service.CreateAsync(
            new CreateOrganizationRequest
            {
                Name = "Organization One",
                Code = "ORG1"
            });

        await Assert.ThrowsAsync<ConflictException>(
            () =>
                service.CreateAsync(
                    new CreateOrganizationRequest
                    {
                        Name = "Organization Two",
                        Code = "ORG1"
                    }));
    }


    [Fact]
    public async Task CreateAsync_WithEmptyName_ShouldThrow()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(
            () =>
                service.CreateAsync(
                    new CreateOrganizationRequest
                    {
                        Name = "",
                        Code = "ORG1"
                    }));
    }


    [Fact]
    public async Task CreateAsync_WithEmptyCode_ShouldThrow()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(
            () =>
                service.CreateAsync(
                    new CreateOrganizationRequest
                    {
                        Name = "Acme",
                        Code = ""
                    }));
    }


    [Fact]
    public async Task UpdateAsync_WithDuplicateCode_ShouldThrow()
    {
        var service = CreateService();

        var first =
            await service.CreateAsync(
                new CreateOrganizationRequest
                {
                    Name = "Organization One",
                    Code = "ORG1"
                });

        await service.CreateAsync(
            new CreateOrganizationRequest
            {
                Name = "Organization Two",
                Code = "ORG2"
            });

        await Assert.ThrowsAsync<ConflictException>(
            () =>
                service.UpdateAsync(
                    first.Id,
                    new UpdateOrganizationRequest
                    {
                        Name = "Organization One Updated",
                        Code = "ORG2"
                    }));
    }


    [Fact]
    public async Task DeactivateAsync_WhenAlreadyInactive_ShouldFail()
    {
        var service = CreateService();

        var created =
            await service.CreateAsync(
                new CreateOrganizationRequest
                {
                    Name = "Acme",
                    Code = "ACME"
                });

        await service.DeactivateAsync(
            created.Id);

        var result =
            await service.DeactivateAsync(
                created.Id);

        Assert.False(
            result.Success);

        Assert.Equal(
            "Organization is already inactive",
            result.Message);
    }
}