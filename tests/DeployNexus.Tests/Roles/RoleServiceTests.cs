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


    [Fact]
    public async Task CreateAsync_ShouldFail_WhenOrganizationDoesNotExist()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();

        var service = new RoleService(
            roleRepository,
            organizationRepository);

        var request = new CreateRoleRequest
        {
            Name = "Administrator",
            Description = "Full system access",
            OrganizationId = Guid.NewGuid()
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => service.CreateAsync(request));
    }


    [Fact]
    public async Task CreateAsync_ShouldFail_WhenOrganizationIsInactive()
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
            Name = "Inactive Organization",
            Code = "INACTIVE",
            IsActive = false
        };

        await organizationRepository.AddAsync(organization);

        var request = new CreateRoleRequest
        {
            Name = "Administrator",
            Description = "Full system access",
            OrganizationId = organization.Id
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => service.CreateAsync(request));
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();

        var service = new RoleService(
            roleRepository,
            organizationRepository);

        var roleId = Guid.NewGuid();

        // Act
        var result = await service.GetByIdAsync(roleId);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();

        var service = new RoleService(
            roleRepository,
            organizationRepository);

        var roleId = Guid.NewGuid();

        var request = new UpdateRoleRequest
        {
            Name = "Updated Role",
            Description = "Updated Description"
        };

        // Act
        var result = await service.UpdateAsync(
            roleId,
            request);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task DeactivateAsync_ShouldFail_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();

        var service = new RoleService(
            roleRepository,
            organizationRepository);

        var roleId = Guid.NewGuid();

        // Act
        var result = await service.DeactivateAsync(roleId);

        // Assert
        Assert.False(result.Success);
    }
}