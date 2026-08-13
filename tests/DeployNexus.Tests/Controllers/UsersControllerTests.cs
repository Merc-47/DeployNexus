using System.Net;
using System.Net.Http.Json;
using DeployNexus.Application.Users.DTOs;
using DeployNexus.Domain.Entities;
using DeployNexus.Tests.Integration;
using DeployNexus.Tests.Repositories;
using DeployNexus.Tests.Roles;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Tests.Controllers;

public class UsersControllerTests
    : IClassFixture<DeployNexusWebApplicationFactory>
{
    private readonly HttpClient _client;

    private readonly FakeUserRepository _userRepository;
    private readonly FakeRoleRepository _roleRepository;
    private readonly FakeOrganizationRepository _organizationRepository;
    private readonly FakePermissionService _permissionService;


    public UsersControllerTests(
        DeployNexusWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        _userRepository =
            factory.Services
                .GetRequiredService<FakeUserRepository>();

        _roleRepository =
            factory.Services
                .GetRequiredService<FakeRoleRepository>();

        _organizationRepository =
            factory.Services
                .GetRequiredService<FakeOrganizationRepository>();

        _permissionService =
            factory.Services
                .GetRequiredService<FakePermissionService>();
    }


    // ============================================================
    // ASSIGN ROLE
    // ============================================================

    [Fact]
    public async Task AssignRole_WhenUserHasPermissionAndRoleIsValid_Returns200()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "USER_ROLE_ASSIGN",
            true);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            userId.ToString());


        var organization = await CreateOrganization();

        var user = await CreateUser(
            organization.Id);


        var role = await CreateRole(
            organization.Id,
            "Administrator",
            true);


        var request = new AssignUserRoleRequest
        {
            RoleId = role.Id
        };


        // Act
        var response =
            await _client.PutAsJsonAsync(
                $"/api/Users/{user.Id}/role",
                request);


        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);


        var updatedUser =
            await _userRepository.GetByIdAsync(user.Id);

        Assert.NotNull(updatedUser);

        Assert.Equal(
            role.Id,
            updatedUser.RoleId);
    }


    // ============================================================
    // DIFFERENT ORGANIZATION
    // ============================================================

    [Fact]
    public async Task AssignRole_WhenRoleBelongsToDifferentOrganization_Returns400()
    {
        // Arrange
        var authenticatedUserId = Guid.NewGuid();

        _permissionService.SetPermission(
            authenticatedUserId,
            "USER_ROLE_ASSIGN",
            true);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            authenticatedUserId.ToString());


        var organization1 =
            await CreateOrganization(
                "Organization One",
                "ORG1");


        var organization2 =
            await CreateOrganization(
                "Organization Two",
                "ORG2");


        var user =
            await CreateUser(
                organization1.Id);


        var role =
            await CreateRole(
                organization2.Id,
                "Administrator",
                true);


        var request = new AssignUserRoleRequest
        {
            RoleId = role.Id
        };


        // Act
        var response =
            await _client.PutAsJsonAsync(
                $"/api/Users/{user.Id}/role",
                request);


        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);


        var updatedUser =
            await _userRepository.GetByIdAsync(user.Id);

        Assert.NotNull(updatedUser);

        Assert.Null(updatedUser.RoleId);
    }


    // ============================================================
    // INACTIVE ROLE
    // ============================================================

    [Fact]
    public async Task AssignRole_WhenRoleIsInactive_Returns400()
    {
        // Arrange
        var authenticatedUserId = Guid.NewGuid();

        _permissionService.SetPermission(
            authenticatedUserId,
            "USER_ROLE_ASSIGN",
            true);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            authenticatedUserId.ToString());


        var organization =
            await CreateOrganization();


        var user =
            await CreateUser(
                organization.Id);


        var role =
            await CreateRole(
                organization.Id,
                "Inactive Role",
                false);


        var request = new AssignUserRoleRequest
        {
            RoleId = role.Id
        };


        // Act
        var response =
            await _client.PutAsJsonAsync(
                $"/api/Users/{user.Id}/role",
                request);


        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);


        var updatedUser =
            await _userRepository.GetByIdAsync(user.Id);

        Assert.NotNull(updatedUser);

        Assert.Null(updatedUser.RoleId);
    }


    // ============================================================
    // REMOVE ROLE
    // ============================================================

    [Fact]
    public async Task AssignRole_WhenRoleIdIsNull_RemovesRoleAndReturns200()
    {
        // Arrange
        var authenticatedUserId = Guid.NewGuid();

        _permissionService.SetPermission(
            authenticatedUserId,
            "USER_ROLE_ASSIGN",
            true);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            authenticatedUserId.ToString());


        var organization =
            await CreateOrganization();


        var role =
            await CreateRole(
                organization.Id,
                "Administrator",
                true);


        var user =
            await CreateUser(
                organization.Id,
                role.Id);


        var request = new AssignUserRoleRequest
        {
            RoleId = null
        };


        // Act
        var response =
            await _client.PutAsJsonAsync(
                $"/api/Users/{user.Id}/role",
                request);


        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);


        var updatedUser =
            await _userRepository.GetByIdAsync(user.Id);

        Assert.NotNull(updatedUser);

        Assert.Null(updatedUser.RoleId);
    }


    // ============================================================
    // NO PERMISSION
    // ============================================================

    [Fact]
    public async Task AssignRole_WhenUserDoesNotHavePermission_Returns403()
    {
        // Arrange
        var authenticatedUserId = Guid.NewGuid();

        _permissionService.SetPermission(
            authenticatedUserId,
            "USER_ROLE_ASSIGN",
            false);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            authenticatedUserId.ToString());


        var organization =
            await CreateOrganization();


        var user =
            await CreateUser(
                organization.Id);


        var role =
            await CreateRole(
                organization.Id,
                "Administrator",
                true);


        var request = new AssignUserRoleRequest
        {
            RoleId = role.Id
        };


        // Act
        var response =
            await _client.PutAsJsonAsync(
                $"/api/Users/{user.Id}/role",
                request);


        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }


    // ============================================================
    // USER NOT FOUND
    // ============================================================

    [Fact]
    public async Task AssignRole_WhenUserDoesNotExist_Returns404()
    {
        // Arrange
        var authenticatedUserId = Guid.NewGuid();

        _permissionService.SetPermission(
            authenticatedUserId,
            "USER_ROLE_ASSIGN",
            true);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            authenticatedUserId.ToString());


        var request = new AssignUserRoleRequest
        {
            RoleId = null
        };


        var nonExistingUserId =
            Guid.NewGuid();


        // Act
        var response =
            await _client.PutAsJsonAsync(
                $"/api/Users/{nonExistingUserId}/role",
                request);


        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    // ============================================================
    // TEST DATA HELPERS
    // ============================================================

    private async Task<Organization> CreateOrganization(
        string name = "DeployNexus",
        string code = "DNX")
    {
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = code,
            IsActive = true
        };

        await _organizationRepository
            .AddAsync(organization);

        return organization;
    }


    private async Task<User> CreateUser(
        Guid organizationId,
        Guid? roleId = null)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = $"user_{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@example.com",
            PasswordHash = "hashed-password",
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
            OrganizationId = organizationId,
            RoleId = roleId
        };

        await _userRepository
            .AddAsync(user);

        return user;
    }


    private async Task<Role> CreateRole(
        Guid organizationId,
        string name,
        bool isActive)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = $"{name} description",
            IsActive = isActive,
            OrganizationId = organizationId
        };

        await _roleRepository
            .AddAsync(role);

        return role;
    }
}