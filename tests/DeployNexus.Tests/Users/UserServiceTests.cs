using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeployNexus.Application.Users.DTOs;
using DeployNexus.Application.Users.Services;
using DeployNexus.Domain.Entities;
using DeployNexus.Tests.Repositories;

namespace DeployNexus.Tests.Users;

public class UserServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var repository = new FakeUserRepository();

        var service = new UserService(repository);

        var organizationId = Guid.NewGuid();

        var request = new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            FirstName = "Jason",
            LastName = "Broody",
            OrganizationId = organizationId
        };


        // Act
        var result = await service.CreateAsync(request);


        // Assert
        Assert.NotNull(result);
        Assert.Equal("jason", result.Username);
        Assert.Equal("jason@test.com", result.Email);
        Assert.True(result.IsActive);
    }
    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var repository = new FakeUserRepository();

        var service = new UserService(repository);

        var organizationId = Guid.NewGuid();

        var createdUser = await service.CreateAsync(new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            FirstName = "Jason",
            LastName = "Broody",
            OrganizationId = organizationId
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
        var repository = new FakeUserRepository();

        var service = new UserService(repository);

        var organizationId = Guid.NewGuid();

        await service.CreateAsync(new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            FirstName = "Jason",
            LastName = "Broody",
            OrganizationId = organizationId
        });

        await service.CreateAsync(new CreateUserRequest
        {
            Username = "admin",
            Email = "admin@test.com",
            FirstName = "Admin",
            LastName = "User",
            OrganizationId = organizationId
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
        var repository = new FakeUserRepository();

        var service = new UserService(repository);

        var organizationId = Guid.NewGuid();

        var createdUser = await service.CreateAsync(new CreateUserRequest
        {
            Username = "jason",
            Email = "old@test.com",
            FirstName = "Jason",
            LastName = "Old",
            OrganizationId = organizationId
        });

        var updateRequest = new UpdateUserRequest
        {
            Username = "jason.updated",
            Email = "new@test.com",
            FirstName = "Jason",
            LastName = "Updated",
            OrganizationId = organizationId
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
        var repository = new FakeUserRepository();

        var service = new UserService(repository);

        var organizationId = Guid.NewGuid();

        var user = await service.CreateAsync(new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            FirstName = "Jason",
            LastName = "Broody",
            OrganizationId = organizationId
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
