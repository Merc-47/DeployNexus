using System.Net;
using System.Net.Http.Json;
using DeployNexus.Application.Permissions.DTOs;
using DeployNexus.Tests.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Tests.Controllers;

public class PermissionsControllerTests
    : IClassFixture<DeployNexusWebApplicationFactory>
{
    private readonly HttpClient _client;

    private readonly FakePermissionService _permissionService;


    public PermissionsControllerTests(
        DeployNexusWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        _permissionService =
            factory.Services
                .GetRequiredService<FakePermissionService>();
    }


    // ============================================================
    // CREATE
    // ============================================================

    [Fact]
    public async Task Create_ShouldReturn201Created()
    {
        // Arrange
        var request = new CreatePermissionRequest
        {
            Name = "Create User",
            Code = "USER_CREATE"
        };


        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/Permissions",
            request);


        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);


        var result =
            await response.Content
                .ReadFromJsonAsync<PermissionDto>();


        Assert.NotNull(result);

        Assert.Equal(
            "Create User",
            result.Name);

        Assert.Equal(
            "USER_CREATE",
            result.Code);

        Assert.True(result.IsActive);
    }


    // ============================================================
    // GET ALL
    // ============================================================

    [Fact]
    public async Task GetAll_ShouldReturn200Ok()
    {
        // Arrange
        var permission =
            await _permissionService.CreateAsync(
                new CreatePermissionRequest
                {
                    Name = "View Users",
                    Code = "USER_VIEW"
                });


        // Act
        var response =
            await _client.GetAsync(
                "/api/Permissions");


        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);


        var permissions =
            await response.Content
                .ReadFromJsonAsync<List<PermissionDto>>();


        Assert.NotNull(permissions);

        Assert.Contains(
            permissions,
            x => x.Id == permission.Id);
    }


    // ============================================================
    // GET BY ID
    // ============================================================

    [Fact]
    public async Task GetById_WhenPermissionExists_ShouldReturn200Ok()
    {
        // Arrange
        var permission =
            await _permissionService.CreateAsync(
                new CreatePermissionRequest
                {
                    Name = "View Users",
                    Code = "USER_VIEW"
                });


        // Act
        var response =
            await _client.GetAsync(
                $"/api/Permissions/{permission.Id}");


        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);


        var result =
            await response.Content
                .ReadFromJsonAsync<PermissionDto>();


        Assert.NotNull(result);

        Assert.Equal(
            permission.Id,
            result.Id);

        Assert.Equal(
            "View Users",
            result.Name);

        Assert.Equal(
            "USER_VIEW",
            result.Code);

        Assert.True(result.IsActive);
    }


    [Fact]
    public async Task GetById_WhenPermissionDoesNotExist_ShouldReturn404NotFound()
    {
        // Act
        var response =
            await _client.GetAsync(
                $"/api/Permissions/{Guid.NewGuid()}");


        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    // ============================================================
    // UPDATE
    // ============================================================

    [Fact]
    public async Task Update_WhenPermissionExists_ShouldReturn200Ok()
    {
        // Arrange
        var permission =
            await _permissionService.CreateAsync(
                new CreatePermissionRequest
                {
                    Name = "Old Permission",
                    Code = "OLD"
                });


        var request = new UpdatePermissionRequest
        {
            Name = "Updated Permission",
            Code = "UPDATED"
        };


        // Act
        var response =
            await _client.PutAsJsonAsync(
                $"/api/Permissions/{permission.Id}",
                request);


        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);


        var result =
            await response.Content
                .ReadFromJsonAsync<PermissionDto>();


        Assert.NotNull(result);

        Assert.Equal(
            permission.Id,
            result.Id);

        Assert.Equal(
            "Updated Permission",
            result.Name);

        Assert.Equal(
            "UPDATED",
            result.Code);

        Assert.True(result.IsActive);
    }


    [Fact]
    public async Task Update_WhenPermissionDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange
        var request = new UpdatePermissionRequest
        {
            Name = "Updated Permission",
            Code = "UPDATED"
        };


        // Act
        var response =
            await _client.PutAsJsonAsync(
                $"/api/Permissions/{Guid.NewGuid()}",
                request);


        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    // ============================================================
    // DELETE / DEACTIVATE
    // ============================================================

    [Fact]
    public async Task Delete_WhenPermissionExists_ShouldReturn204NoContent()
    {
        // Arrange
        var permission =
            await _permissionService.CreateAsync(
                new CreatePermissionRequest
                {
                    Name = "Delete Permission",
                    Code = "DELETE_PERMISSION"
                });


        // Act
        var response =
            await _client.DeleteAsync(
                $"/api/Permissions/{permission.Id}");


        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);


        // Verify that DELETE actually deactivated
        // the permission instead of removing it.
        var deactivated =
            await _permissionService
                .GetByIdAsync(permission.Id);


        Assert.NotNull(deactivated);

        Assert.False(
            deactivated.IsActive);
    }


    [Fact]
    public async Task Delete_WhenPermissionDoesNotExist_ShouldReturn404NotFound()
    {
        // Act
        var response =
            await _client.DeleteAsync(
                $"/api/Permissions/{Guid.NewGuid()}");


        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}