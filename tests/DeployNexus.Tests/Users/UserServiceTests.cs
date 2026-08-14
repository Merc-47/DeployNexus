using DeployNexus.Application.Users.DTOs;
using DeployNexus.Application.Users.Services;
using DeployNexus.Domain.Entities;
using DeployNexus.Tests.Authentication;
using DeployNexus.Tests.Repositories;
using DeployNexus.Tests.Roles;

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

        var roleRepository = new FakeRoleRepository();

        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);


        var request = new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            Password = "Password123!",
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

        // User should initially have no role.
        Assert.Null(result.RoleId);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out var organization);

        var roleRepository = new FakeRoleRepository();

        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);


        var createdUser = await service.CreateAsync(
            new CreateUserRequest
            {
                Username = "jason",
                Email = "jason@test.com",
                Password = "Password123!",
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
        Assert.Null(result.RoleId);
    }


    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out var organization);

        var roleRepository = new FakeRoleRepository();

        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);


        await service.CreateAsync(
            new CreateUserRequest
            {
                Username = "jason",
                Email = "jason@test.com",
                Password = "Password123!",
                FirstName = "Jason",
                LastName = "Broody",
                OrganizationId = organization.Id
            });


        await service.CreateAsync(
            new CreateUserRequest
            {
                Username = "admin",
                Email = "admin@test.com",
                Password = "Password123!",
                FirstName = "Admin",
                LastName = "User",
                OrganizationId = organization.Id
            });


        // Act
        var result = await service.GetAllAsync();


        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        Assert.All(
            result,
            user => Assert.Null(user.RoleId));
    }


    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out var organization);

        var roleRepository = new FakeRoleRepository();

        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);


        var createdUser = await service.CreateAsync(
            new CreateUserRequest
            {
                Username = "jason",
                Email = "old@test.com",
                Password = "Password123!",
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

        // Role should remain unchanged.
        Assert.Null(result.RoleId);
    }


    [Fact]
    public async Task DeactivateAsync_ShouldDeactivateUser_WhenUserExists()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out var organization);

        var roleRepository = new FakeRoleRepository();

        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);


        var user = await service.CreateAsync(
            new CreateUserRequest
            {
                Username = "jason",
                Email = "jason@test.com",
                Password = "Password123!",
                FirstName = "Jason",
                LastName = "Broody",
                OrganizationId = organization.Id
            });


        // Act
        var result =
            await service.DeactivateAsync(user.Id);

        var updatedUser =
            await service.GetByIdAsync(user.Id);


        // Assert
        Assert.True(result.Success);
        Assert.NotNull(updatedUser);
        Assert.False(updatedUser.IsActive);

        // Role should remain unchanged.
        Assert.Null(updatedUser.RoleId);
    }


    [Fact]
    public async Task CreateAsync_ShouldFail_WhenOrganizationDoesNotExist()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            new FakeOrganizationRepository();

        var roleRepository = new FakeRoleRepository();

        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);


        var request = new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            Password = "Password123!",
            FirstName = "Jason",
            LastName = "Broody",
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
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            new FakeOrganizationRepository();

        var roleRepository = new FakeRoleRepository();

        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);


        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Organization",
            Code = "INACTIVE",
            IsActive = false
        };


        organizationRepository.AddTestOrganization(
            organization);


        var request = new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            Password = "Password123!",
            FirstName = "Jason",
            LastName = "Broody",
            OrganizationId = organization.Id
        };


        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => service.CreateAsync(request));
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out _);

        var roleRepository = new FakeRoleRepository();

        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);


        // Act
        var result =
            await service.GetByIdAsync(Guid.NewGuid());


        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out var organization);

        var roleRepository = new FakeRoleRepository();

        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);


        var request = new UpdateUserRequest
        {
            Username = "updated",
            Email = "updated@test.com",
            FirstName = "Updated",
            LastName = "User",
            OrganizationId = organization.Id
        };


        // Act
        var result = await service.UpdateAsync(
            Guid.NewGuid(),
            request);


        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task DeactivateAsync_ShouldFail_WhenUserDoesNotExist()
    {
        // Arrange
        var userRepository = new FakeUserRepository();

        var organizationRepository =
            CreateOrganizationRepository(out _);

        var roleRepository = new FakeRoleRepository();

        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);


        // Act
        var result =
            await service.DeactivateAsync(Guid.NewGuid());


        // Assert
        Assert.False(result.Success);
        Assert.Equal(
            "User not found",
            result.Message);
    }

    [Fact]
    public async Task AssignRoleAsync_ShouldAssignActiveRoleFromSameOrganization()
    {
        // Arrange
        var userRepository = new FakeUserRepository();
        var organizationRepository = new FakeOrganizationRepository();
        var roleRepository = new FakeRoleRepository();
        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "DeployNexus",
            Code = "DNX",
            IsActive = true
        };

        await organizationRepository.AddAsync(organization);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "john",
            Email = "john@deploynexus.com",
            PasswordHash = "hashed",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true,
            OrganizationId = organization.Id,
            RoleId = null
        };

        await userRepository.AddAsync(user);

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            Description = "Full access",
            IsActive = true,
            OrganizationId = organization.Id
        };

        await roleRepository.AddAsync(role);

        var request = new AssignUserRoleRequest
        {
            RoleId = role.Id
        };

        // Act
        var result = await service.AssignRoleAsync(
            user.Id,
            request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(role.Id, result.RoleId);
    }

    [Fact]
    public async Task AssignRoleAsync_ShouldFail_WhenRoleBelongsToDifferentOrganization()
    {
        // Arrange
        var userRepository = new FakeUserRepository();
        var organizationRepository = new FakeOrganizationRepository();
        var roleRepository = new FakeRoleRepository();
        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);

        var organization1 = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Organization One",
            Code = "ORG1",
            IsActive = true
        };

        var organization2 = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Organization Two",
            Code = "ORG2",
            IsActive = true
        };

        await organizationRepository.AddAsync(organization1);
        await organizationRepository.AddAsync(organization2);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "john",
            Email = "john@example.com",
            PasswordHash = "hashed",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true,
            OrganizationId = organization1.Id,
            RoleId = null
        };

        await userRepository.AddAsync(user);

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            Description = "Admin",
            IsActive = true,
            OrganizationId = organization2.Id
        };

        await roleRepository.AddAsync(role);

        var request = new AssignUserRoleRequest
        {
            RoleId = role.Id
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.AssignRoleAsync(
                user.Id,
                request));

        Assert.Equal(
            "Role does not belong to the user's organization",
            exception.Message);
    }

    [Fact]
    public async Task AssignRoleAsync_ShouldFail_WhenRoleIsInactive()
    {
        // Arrange
        var userRepository = new FakeUserRepository();
        var organizationRepository = new FakeOrganizationRepository();
        var roleRepository = new FakeRoleRepository();
        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "DeployNexus",
            Code = "DNX",
            IsActive = true
        };

        await organizationRepository.AddAsync(organization);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "john",
            Email = "john@example.com",
            PasswordHash = "hashed",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true,
            OrganizationId = organization.Id,
            RoleId = null
        };

        await userRepository.AddAsync(user);

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            Description = "Admin",
            IsActive = false,
            OrganizationId = organization.Id
        };

        await roleRepository.AddAsync(role);

        var request = new AssignUserRoleRequest
        {
            RoleId = role.Id
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.AssignRoleAsync(
                user.Id,
                request));

        Assert.Equal(
            "Role is inactive",
            exception.Message);


    }

    [Fact]
    public async Task AssignRoleAsync_ShouldRemoveRole_WhenRoleIdIsNull()
    {
        // Arrange
        var userRepository = new FakeUserRepository();
        var organizationRepository = new FakeOrganizationRepository();
        var roleRepository = new FakeRoleRepository();
        var passwordHasher = new FakePasswordHasher();

        var service = new UserService(
            userRepository,
            organizationRepository,
            roleRepository,
            passwordHasher);

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "DeployNexus",
            Code = "DNX",
            IsActive = true
        };

        await organizationRepository.AddAsync(organization);

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            Description = "Admin",
            IsActive = true,
            OrganizationId = organization.Id
        };

        await roleRepository.AddAsync(role);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "john",
            Email = "john@example.com",
            PasswordHash = "hashed",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true,
            OrganizationId = organization.Id,
            RoleId = role.Id
        };

        await userRepository.AddAsync(user);

        var request = new AssignUserRoleRequest
        {
            RoleId = null
        };

        // Act
        var result = await service.AssignRoleAsync(
            user.Id,
            request);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.RoleId);
    }
}