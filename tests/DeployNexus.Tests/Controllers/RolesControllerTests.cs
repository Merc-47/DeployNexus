using System.Net;
using System.Net.Http.Json;
using DeployNexus.Tests.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Tests.Controllers;

public class RolesControllerTests
    : IClassFixture<DeployNexusWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly FakePermissionService _permissionService;
    private readonly FakeRoleService _roleService;

    public RolesControllerTests(
        DeployNexusWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        _permissionService =
            factory.Services
                .GetRequiredService<FakePermissionService>();

        _roleService =
            factory.Services
                .GetRequiredService<FakeRoleService>();
    }


    [Fact]
    public async Task GetAll_WhenUserHasRoleViewPermission_Returns200()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_VIEW",
            true);

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/Roles");

        request.Headers.Add(
            "X-Test-UserId",
            userId.ToString());


        // Act
        var response =
            await _client.SendAsync(request);


        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    [Fact]
    public async Task GetAll_WhenUserDoesNotHaveRoleViewPermission_Returns403()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_VIEW",
            false);

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/Roles");

        request.Headers.Add(
            "X-Test-UserId",
            userId.ToString());


        // Act
        var response =
            await _client.SendAsync(request);


        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }


    [Fact]
    public async Task GetById_WhenUserHasRoleViewPermission_Returns200()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_VIEW",
            true);

        var role =
            await _roleService.CreateAsync(
                new Application.Roles.DTOs.CreateRoleRequest
                {
                    Name = "Test Role",
                    Description = "Integration Test Role",
                    OrganizationId = Guid.NewGuid()
                });

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/Roles/{role.Id}");

        request.Headers.Add(
            "X-Test-UserId",
            userId.ToString());


        // Act
        var response =
            await _client.SendAsync(request);


        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    [Fact]
    public async Task Create_WhenUserHasRoleCreatePermission_Returns201()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_CREATE",
            true);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/Roles");

        request.Headers.Add(
            "X-Test-UserId",
            userId.ToString());

        request.Content = JsonContent.Create(
            new
            {
                name = "Created Role",
                description = "Created by integration test",
                organizationId = Guid.NewGuid()
            });


        // Act
        var response =
            await _client.SendAsync(request);


        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }


    [Fact]
    public async Task Create_WhenUserDoesNotHaveRoleCreatePermission_Returns403()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_CREATE",
            false);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/Roles");

        request.Headers.Add(
            "X-Test-UserId",
            userId.ToString());

        request.Content = JsonContent.Create(
            new
            {
                name = "Unauthorized Role",
                description = "Should not be created",
                organizationId = Guid.NewGuid()
            });


        // Act
        var response =
            await _client.SendAsync(request);


        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }


    [Fact]
    public async Task Update_WhenUserHasRoleUpdatePermission_Returns200()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_UPDATE",
            true);

        var role =
            await _roleService.CreateAsync(
                new Application.Roles.DTOs.CreateRoleRequest
                {
                    Name = "Original Role",
                    Description = "Original Description",
                    OrganizationId = Guid.NewGuid()
                });

        var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"/api/Roles/{role.Id}");

        request.Headers.Add(
            "X-Test-UserId",
            userId.ToString());

        request.Content = JsonContent.Create(
            new
            {
                name = "Updated Role",
                description = "Updated Description"
            });


        // Act
        var response =
            await _client.SendAsync(request);


        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    [Fact]
    public async Task Update_WhenUserDoesNotHaveRoleUpdatePermission_Returns403()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_UPDATE",
            false);

        var role =
            await _roleService.CreateAsync(
                new Application.Roles.DTOs.CreateRoleRequest
                {
                    Name = "Original Role",
                    Description = "Original Description",
                    OrganizationId = Guid.NewGuid()
                });

        var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"/api/Roles/{role.Id}");

        request.Headers.Add(
            "X-Test-UserId",
            userId.ToString());

        request.Content = JsonContent.Create(
            new
            {
                name = "Should Not Update",
                description = "Should Not Update"
            });


        // Act
        var response =
            await _client.SendAsync(request);


        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenUserHasRoleDeletePermission_Returns204()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_DELETE",
            true);

        var role =
            await _roleService.CreateAsync(
                new Application.Roles.DTOs.CreateRoleRequest
                {
                    Name = "Role To Delete",
                    Description = "Delete Test",
                    OrganizationId = Guid.NewGuid()
                });

        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/Roles/{role.Id}");

        request.Headers.Add(
            "X-Test-UserId",
            userId.ToString());


        // Act
        var response =
            await _client.SendAsync(request);


        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }


    [Fact]
    public async Task Delete_WhenUserDoesNotHaveRoleDeletePermission_Returns403()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _permissionService.SetPermission(
            userId,
            "ROLE_DELETE",
            false);

        var role =
            await _roleService.CreateAsync(
                new Application.Roles.DTOs.CreateRoleRequest
                {
                    Name = "Protected Role",
                    Description = "Should not delete",
                    OrganizationId = Guid.NewGuid()
                });

        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/Roles/{role.Id}");

        request.Headers.Add(
            "X-Test-UserId",
            userId.ToString());


        // Act
        var response =
            await _client.SendAsync(request);


        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}