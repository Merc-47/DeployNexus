using DeployNexus.Application.Roles.DTOs;
using DeployNexus.Application.Roles.Services;
using DeployNexus.Tests.Organizations;
using DeployNexus.Tests.Repositories;

namespace DeployNexus.Tests.Roles;

public class RoleServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateRoleSuccessfully()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();

        var service = new RoleService(
            roleRepository,
            organizationRepository);


        var organization = new Domain.Entities.Organization
        {
            Id = Guid.NewGuid(),
            Name = "DeployNexus",
            Code = "DNX",
            IsActive = true
        };


        await organizationRepository.AddAsync(organization);


        var request = new CreateRoleRequest
        {
            Name = "Administrator",
            Description = "Full system access",
            OrganizationId = organization.Id
        };


        // Act
        var result = await service.CreateAsync(request);


        // Assert
        Assert.NotNull(result);
        Assert.Equal("Administrator", result.Name);
        Assert.Equal("Full system access", result.Description);
        Assert.True(result.IsActive);
        Assert.Equal(organization.Id, result.OrganizationId);
    }



    [Fact]
    public async Task GetByIdAsync_ShouldReturnRole_WhenRoleExists()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();

        var service = new RoleService(
            roleRepository,
            organizationRepository);


        var organization = new Domain.Entities.Organization
        {
            Id = Guid.NewGuid(),
            Name = "DeployNexus",
            Code = "DNX",
            IsActive = true
        };

        await organizationRepository.AddAsync(organization);


        var created = await service.CreateAsync(new CreateRoleRequest
        {
            Name = "Admin",
            Description = "Access",
            OrganizationId = organization.Id
        });


        // Act
        var result = await service.GetByIdAsync(created.Id);


        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Admin", result.Name);
    }



    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRoles()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();

        var service = new RoleService(
            roleRepository,
            organizationRepository);


        var organization = new Domain.Entities.Organization
        {
            Id = Guid.NewGuid(),
            Name = "DeployNexus",
            Code = "DNX",
            IsActive = true
        };

        await organizationRepository.AddAsync(organization);


        await service.CreateAsync(new CreateRoleRequest
        {
            Name = "Admin",
            Description = "Admin Access",
            OrganizationId = organization.Id
        });


        await service.CreateAsync(new CreateRoleRequest
        {
            Name = "User",
            Description = "User Access",
            OrganizationId = organization.Id
        });


        // Act
        var result = await service.GetAllAsync();


        // Assert
        Assert.Equal(2, result.Count());
    }



    [Fact]
    public async Task UpdateAsync_ShouldUpdateRole_WhenRoleExists()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();

        var service = new RoleService(
            roleRepository,
            organizationRepository);


        var organization = new Domain.Entities.Organization
        {
            Id = Guid.NewGuid(),
            Name = "DeployNexus",
            Code = "DNX",
            IsActive = true
        };

        await organizationRepository.AddAsync(organization);


        var created = await service.CreateAsync(new CreateRoleRequest
        {
            Name = "Admin",
            Description = "Old Description",
            OrganizationId = organization.Id
        });


        // Act
        var result = await service.UpdateAsync(
            created.Id,
            new UpdateRoleRequest
            {
                Name = "Administrator",
                Description = "Updated Description"
            });


        // Assert
        Assert.NotNull(result);
        Assert.Equal("Administrator", result.Name);
        Assert.Equal("Updated Description", result.Description);
    }



    [Fact]
    public async Task DeactivateAsync_ShouldDeactivateRole_WhenRoleExists()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();

        var service = new RoleService(
            roleRepository,
            organizationRepository);


        var organization = new Domain.Entities.Organization
        {
            Id = Guid.NewGuid(),
            Name = "DeployNexus",
            Code = "DNX",
            IsActive = true
        };

        await organizationRepository.AddAsync(organization);


        var created = await service.CreateAsync(new CreateRoleRequest
        {
            Name = "Admin",
            Description = "Access",
            OrganizationId = organization.Id
        });


        // Act
        var result = await service.DeactivateAsync(created.Id);

        var role = await service.GetByIdAsync(created.Id);


        // Assert
        Assert.True(result.Success);
        Assert.NotNull(role);
        Assert.False(role.IsActive);
    }
}