using DeployNexus.Application.Users.DTOs;
using DeployNexus.Application.Users.Services;
using DeployNexus.Domain.Entities;
using DeployNexus.Tests.Repositories;

namespace DeployNexus.Tests.Users;

public class UserServiceTests
{
    private static FakeOrganizationRepository CreateOrganizationRepository(
        out Organization organization)
    {
        var repository = new FakeOrganizationRepository();

        organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "DeployNexus Internal",
            Code = "DNX",
            IsActive = true
        };

        repository.AddTestOrganization(organization);

        return repository;
    }


    [Fact]
    public async Task CreateAsync_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out var organization);

        var service = new UserService(
            userRepository,
            organizationRepository
        );


        var request = new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            FirstName = "Jason",
            LastName = "Broody",
            OrganizationId = organization.Id
        };


        // Act
        var result = await service.CreateAsync(request);


        // Assert
        Assert.NotNull(result);
        Assert.Equal("jason", result.Username);
        Assert.Equal("jason@test.com", result.Email);
        Assert.Equal(organization.Id, result.OrganizationId);
        Assert.True(result.IsActive);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out var organization);

        var service = new UserService(
            userRepository,
            organizationRepository
        );


        var createdUser = await service.CreateAsync(new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            FirstName = "Jason",
            LastName = "Broody",
            OrganizationId = organization.Id
        });


        // Act
        var result = await service.GetByIdAsync(createdUser.Id);


        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdUser.Id, result.Id);
        Assert.Equal("jason", result.Username);
    }


    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out var organization);

        var service = new UserService(
            userRepository,
            organizationRepository
        );


        await service.CreateAsync(new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            FirstName = "Jason",
            LastName = "Broody",
            OrganizationId = organization.Id
        });


        await service.CreateAsync(new CreateUserRequest
        {
            Username = "admin",
            Email = "admin@test.com",
            FirstName = "Admin",
            LastName = "User",
            OrganizationId = organization.Id
        });


        // Act
        var result = await service.GetAllAsync();


        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }


    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out var organization);

        var service = new UserService(
            userRepository,
            organizationRepository
        );


        var createdUser = await service.CreateAsync(new CreateUserRequest
        {
            Username = "jason",
            Email = "old@test.com",
            FirstName = "Jason",
            LastName = "Old",
            OrganizationId = organization.Id
        });


        var updateRequest = new UpdateUserRequest
        {
            Username = "jason.updated",
            Email = "new@test.com",
            FirstName = "Jason",
            LastName = "Updated",
            OrganizationId = organization.Id
        };


        // Act
        var result = await service.UpdateAsync(
            createdUser.Id,
            updateRequest);


        // Assert
        Assert.NotNull(result);
        Assert.Equal("jason.updated", result.Username);
        Assert.Equal("new@test.com", result.Email);
        Assert.Equal("Updated", result.LastName);
    }


    [Fact]
    public async Task DeactivateAsync_ShouldDeactivateUser_WhenUserExists()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out var organization);

        var service = new UserService(
            userRepository,
            organizationRepository
        );


        var user = await service.CreateAsync(new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            FirstName = "Jason",
            LastName = "Broody",
            OrganizationId = organization.Id
        });


        // Act
        var result = await service.DeactivateAsync(user.Id);

        var updatedUser = await service.GetByIdAsync(user.Id);


        // Assert
        Assert.True(result.Success);
        Assert.NotNull(updatedUser);
        Assert.False(updatedUser.IsActive);
    }
}