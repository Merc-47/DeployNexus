using System.Net;
using System.Net.Http.Json;
using DeployNexus.Application.RolePermissions.DTOs;
using DeployNexus.Domain.Entities;
using DeployNexus.Tests.Integration;
using DeployNexus.Tests.Repositories;
using DeployNexus.Tests.Roles;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Tests.Controllers;

public class RolePermissionsControllerTests
    : IClassFixture<DeployNexusWebApplicationFactory>
{
    private readonly HttpClient _client;

    private readonly FakePermissionService _permissionService;
    private readonly FakeRoleService _roleService;

    private readonly FakeRoleRepository _roleRepository;
    private readonly FakePermissionRepository _permissionRepository;
    private readonly FakeRolePermissionRepository
        _rolePermissionRepository;


    public RolePermissionsControllerTests(
        DeployNexusWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        _permissionService =
            factory.Services
                .GetRequiredService<FakePermissionService>();

        _roleService =
            factory.Services
                .GetRequiredService<FakeRoleService>();

        _roleRepository =
            factory.Services
                .GetRequiredService<FakeRoleRepository>();

        _permissionRepository =
            factory.Services
                .GetRequiredService<FakePermissionRepository>();

        _rolePermissionRepository =
            factory.Services
                .GetRequiredService<FakeRolePermissionRepository>();
    }


    // ============================================================
    // GET PERMISSIONS
    // ============================================================

    [Fact]
    public async Task GetPermissions_WhenUserHasViewPermission_Returns200()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_PERMISSION_VIEW",
            true);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            userId.ToString());

        var role = await CreateRole();


        // Act
        var response =
            await _client.GetAsync(
                $"/api/Roles/{role.Id}/permissions");


        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    [Fact]
    public async Task GetPermissions_WhenUserDoesNotHaveViewPermission_Returns403()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_PERMISSION_VIEW",
            false);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            userId.ToString());

        var role = await CreateRole();


        // Act
        var response =
            await _client.GetAsync(
                $"/api/Roles/{role.Id}/permissions");


        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }


    // ============================================================
    // ASSIGN
    // ============================================================

    [Fact]
    public async Task Assign_WhenUserHasAssignPermission_Returns201()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_PERMISSION_ASSIGN",
            true);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            userId.ToString());

        var role = await CreateRole();

        var permission = await CreatePermission();

        var request = new AssignPermissionRequest
        {
            PermissionId = permission.Id
        };


        // Act
        var response =
            await _client.PostAsJsonAsync(
                $"/api/Roles/{role.Id}/permissions",
                request);


        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }


    [Fact]
    public async Task Assign_WhenUserDoesNotHaveAssignPermission_Returns403()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_PERMISSION_ASSIGN",
            false);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            userId.ToString());

        var role = await CreateRole();

        var permission = await CreatePermission();

        var request = new AssignPermissionRequest
        {
            PermissionId = permission.Id
        };


        // Act
        var response =
            await _client.PostAsJsonAsync(
                $"/api/Roles/{role.Id}/permissions",
                request);


        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }


    // ============================================================
    // REMOVE
    // ============================================================

    [Fact]
    public async Task Remove_WhenUserHasRemovePermission_Returns204()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_PERMISSION_REMOVE",
            true);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            userId.ToString());

        var role = await CreateRole();

        var permission = await CreatePermission();


        // Create existing role-permission assignment
        var rolePermission = new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = role.Id,
            PermissionId = permission.Id
        };

        await _rolePermissionRepository.AddAsync(
            rolePermission);


        // Act
        var response =
            await _client.DeleteAsync(
                $"/api/Roles/{role.Id}/permissions/{permission.Id}");


        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }


    [Fact]
    public async Task Remove_WhenUserDoesNotHaveRemovePermission_Returns403()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_PERMISSION_REMOVE",
            false);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            userId.ToString());

        var role = await CreateRole();

        var permission = await CreatePermission();


        // Create existing assignment
        var rolePermission = new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = role.Id,
            PermissionId = permission.Id
        };

        await _rolePermissionRepository.AddAsync(
            rolePermission);


        // Act
        var response =
            await _client.DeleteAsync(
                $"/api/Roles/{role.Id}/permissions/{permission.Id}");


        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }


    [Fact]
    public async Task Remove_WhenAssignmentDoesNotExist_Returns404()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_PERMISSION_REMOVE",
            true);

        _client.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            userId.ToString());

        var role = await CreateRole();

        var permission = await CreatePermission();


        // No RolePermission assignment is created here.


        // Act
        var response =
            await _client.DeleteAsync(
                $"/api/Roles/{role.Id}/permissions/{permission.Id}");


        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    // ============================================================
    // TEST DATA HELPERS
    // ============================================================

    private async Task<Role> CreateRole()
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Test Role",
            Description = "Integration Test Role",
            IsActive = true,
            OrganizationId = Guid.NewGuid()
        };

        await _roleRepository.AddAsync(role);

        return role;
    }


    private async Task<Permission> CreatePermission()
    {
        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "View Roles",
            Code = "ROLE_VIEW",
            IsActive = true
        };

        await _permissionRepository.AddAsync(
            permission);

        return permission;
    }
}