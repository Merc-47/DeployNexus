using DeployNexus.Application.Common.Exceptions;
using DeployNexus.Application.Roles.DTOs;
using DeployNexus.Application.Roles.Services;
using DeployNexus.Tests.Repositories;

namespace DeployNexus.Tests.Roles;

public class RoleServiceTests
{
    private static (
        FakeRoleRepository RoleRepository,
        FakeOrganizationRepository OrganizationRepository,
        RoleService Service)
        CreateService()
    {
        var roleRepository =
            new FakeRoleRepository();

        var organizationRepository =
            new FakeOrganizationRepository();

        var service =
            new RoleService(
                roleRepository,
                organizationRepository);

        return (
            roleRepository,
            organizationRepository,
            service);
    }


    private static Domain.Entities.Organization
        CreateOrganization(
            FakeOrganizationRepository repository,
            string name = "DeployNexus",
            string code = "DNX",
            bool isActive = true)
    {
        var organization =
            new Domain.Entities.Organization
            {
                Id = Guid.NewGuid(),
                Name = name,
                Code = code,
                IsActive = isActive
            };

        repository.AddTestOrganization(
            organization);

        return organization;
    }


    [Fact]
    public async Task CreateAsync_ShouldCreateRoleSuccessfully()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();

        var organization =
            CreateOrganization(
                organizationRepository);

        var request =
            new CreateRoleRequest
            {
                Name = "Administrator",
                Description = "Full system access",
                OrganizationId = organization.Id
            };


        var result =
            await service.CreateAsync(request);


        Assert.NotNull(result);
        Assert.Equal(
            "Administrator",
            result.Name);

        Assert.Equal(
            "Full system access",
            result.Description);

        Assert.True(result.IsActive);

        Assert.Equal(
            organization.Id,
            result.OrganizationId);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnRole_WhenRoleExists()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();

        var organization =
            CreateOrganization(
                organizationRepository);

        var created =
            await service.CreateAsync(
                new CreateRoleRequest
                {
                    Name = "Admin",
                    Description = "Access",
                    OrganizationId = organization.Id
                });


        var result =
            await service.GetByIdAsync(
                created.Id);


        Assert.NotNull(result);
        Assert.Equal(
            created.Id,
            result.Id);

        Assert.Equal(
            "Admin",
            result.Name);
    }


    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRoles()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();

        var organization =
            CreateOrganization(
                organizationRepository);


        await service.CreateAsync(
            new CreateRoleRequest
            {
                Name = "Admin",
                Description = "Admin Access",
                OrganizationId = organization.Id
            });


        await service.CreateAsync(
            new CreateRoleRequest
            {
                Name = "User",
                Description = "User Access",
                OrganizationId = organization.Id
            });


        var result =
            await service.GetAllAsync();


        Assert.Equal(
            2,
            result.Count());
    }


    [Fact]
    public async Task UpdateAsync_ShouldUpdateRole_WhenRoleExists()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();

        var organization =
            CreateOrganization(
                organizationRepository);


        var created =
            await service.CreateAsync(
                new CreateRoleRequest
                {
                    Name = "Admin",
                    Description = "Old Description",
                    OrganizationId = organization.Id
                });


        var result =
            await service.UpdateAsync(
                created.Id,
                new UpdateRoleRequest
                {
                    Name = "Administrator",
                    Description = "Updated Description"
                });


        Assert.NotNull(result);

        Assert.Equal(
            "Administrator",
            result.Name);

        Assert.Equal(
            "Updated Description",
            result.Description);
    }


    [Fact]
    public async Task DeactivateAsync_ShouldDeactivateRole_WhenRoleExists()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();

        var organization =
            CreateOrganization(
                organizationRepository);


        var created =
            await service.CreateAsync(
                new CreateRoleRequest
                {
                    Name = "Admin",
                    Description = "Access",
                    OrganizationId = organization.Id
                });


        var result =
            await service.DeactivateAsync(
                created.Id);

        var role =
            await service.GetByIdAsync(
                created.Id);


        Assert.True(result.Success);
        Assert.NotNull(role);
        Assert.False(role.IsActive);
    }


    [Fact]
    public async Task CreateAsync_ShouldThrowNotFoundException_WhenOrganizationDoesNotExist()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();


        var request =
            new CreateRoleRequest
            {
                Name = "Administrator",
                Description = "Full system access",
                OrganizationId = Guid.NewGuid()
            };


        var exception =
            await Assert.ThrowsAsync<NotFoundException>(
                () => service.CreateAsync(request));


        Assert.Equal(
            "Organization not found",
            exception.Message);
    }


    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenOrganizationIsInactive()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();


        var organization =
            CreateOrganization(
                organizationRepository,
                "Inactive Organization",
                "INACTIVE",
                false);


        var request =
            new CreateRoleRequest
            {
                Name = "Administrator",
                Description = "Full system access",
                OrganizationId = organization.Id
            };


        var exception =
            await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(request));


        Assert.Equal(
            "Organization is inactive",
            exception.Message);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenRoleDoesNotExist()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();


        var result =
            await service.GetByIdAsync(
                Guid.NewGuid());


        Assert.Null(result);
    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenRoleDoesNotExist()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();


        var request =
            new UpdateRoleRequest
            {
                Name = "Updated Role",
                Description = "Updated Description"
            };


        var result =
            await service.UpdateAsync(
                Guid.NewGuid(),
                request);


        Assert.Null(result);
    }


    [Fact]
    public async Task DeactivateAsync_ShouldFail_WhenRoleDoesNotExist()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();


        var result =
            await service.DeactivateAsync(
                Guid.NewGuid());


        Assert.False(result.Success);

        Assert.Equal(
            "Role not found",
            result.Message);
    }


    [Fact]
    public async Task DeactivateAsync_ShouldFail_WhenRoleIsAlreadyInactive()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();

        var organization =
            CreateOrganization(
                organizationRepository);


        var created =
            await service.CreateAsync(
                new CreateRoleRequest
                {
                    Name = "Admin",
                    Description = "Access",
                    OrganizationId = organization.Id
                });


        await service.DeactivateAsync(
            created.Id);


        var result =
            await service.DeactivateAsync(
                created.Id);


        Assert.False(result.Success);

        Assert.Equal(
            "Role is already inactive",
            result.Message);
    }


    [Fact]
    public async Task CreateAsync_ShouldThrowConflictException_WhenRoleNameAlreadyExistsInOrganization()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();

        var organization =
            CreateOrganization(
                organizationRepository);


        var request =
            new CreateRoleRequest
            {
                Name = "Administrator",
                Description = "Full system access",
                OrganizationId = organization.Id
            };


        await service.CreateAsync(
            request);


        var exception =
            await Assert.ThrowsAsync<ConflictException>(
                () => service.CreateAsync(request));


        Assert.Equal(
            "A role with this name already exists in the organization",
            exception.Message);
    }


    [Fact]
    public async Task CreateAsync_ShouldAllowSameRoleNameInDifferentOrganizations()
    {
        var (
            roleRepository,
            organizationRepository,
            service) = CreateService();


        var organization1 =
            CreateOrganization(
                organizationRepository,
                "Organization One",
                "ORG1");


        var organization2 =
            CreateOrganization(
                organizationRepository,
                "Organization Two",
                "ORG2");


        var role1 =
            await service.CreateAsync(
                new CreateRoleRequest
                {
                    Name = "Administrator",
                    Description = "Admin for Organization One",
                    OrganizationId = organization1.Id
                });


        var role2 =
            await service.CreateAsync(
                new CreateRoleRequest
                {
                    Name = "Administrator",
                    Description = "Admin for Organization Two",
                    OrganizationId = organization2.Id
                });


        Assert.NotNull(role1);
        Assert.NotNull(role2);

        Assert.Equal(
            "Administrator",
            role1.Name);

        Assert.Equal(
            "Administrator",
            role2.Name);

        Assert.Equal(
            organization1.Id,
            role1.OrganizationId);

        Assert.Equal(
            organization2.Id,
            role2.OrganizationId);

        Assert.NotEqual(
            role1.Id,
            role2.Id);
    }
}