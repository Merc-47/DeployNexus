using System.Net;
using System.Net.Http.Json;
using DeployNexus.Application.Modules.DTOs;
using DeployNexus.Tests.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Tests.Controllers;

public class ModulesControllerTests
    : IClassFixture<DeployNexusWebApplicationFactory>
{
    private readonly HttpClient _client;

    private readonly FakeModuleService _moduleService;

    public ModulesControllerTests(
        DeployNexusWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        _moduleService =
            factory.Services
                .GetRequiredService<FakeModuleService>();
    }


    // ============================================================
    // CREATE
    // ============================================================

    [Fact]
    public async Task Create_ShouldReturn201Created()
    {
        // Arrange

        var request = new CreateModuleRequest
        {
            Name = "User Management",
            Code = "USER_MANAGEMENT",
            Description = "User management module"
        };


        // Act

        var response = await _client.PostAsJsonAsync(
            "/api/Modules",
            request);


        // Assert

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }


    // ============================================================
    // GET ALL
    // ============================================================

    [Fact]
    public async Task GetAll_ShouldReturn200Ok()
    {
        // Act

        var response = await _client.GetAsync(
            "/api/Modules");


        // Assert

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    // ============================================================
    // GET BY ID - EXISTS
    // ============================================================

    [Fact]
    public async Task GetById_WhenModuleExists_ShouldReturn200Ok()
    {
        // Arrange

        var module =
            await _moduleService.CreateAsync(
                new CreateModuleRequest
                {
                    Name = "User Management",
                    Code = "USER_MANAGEMENT",
                    Description = "User management module"
                });


        // Act

        var response = await _client.GetAsync(
            $"/api/Modules/{module.Id}");


        // Assert

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    // ============================================================
    // GET BY ID - DOES NOT EXIST
    // ============================================================

    [Fact]
    public async Task GetById_WhenModuleDoesNotExist_ShouldReturn404NotFound()
    {
        // Act

        var response = await _client.GetAsync(
            $"/api/Modules/{Guid.NewGuid()}");


        // Assert

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    // ============================================================
    // UPDATE - EXISTS
    // ============================================================

    [Fact]
    public async Task Update_WhenModuleExists_ShouldReturn200Ok()
    {
        // Arrange

        var module =
            await _moduleService.CreateAsync(
                new CreateModuleRequest
                {
                    Name = "Old Module",
                    Code = "OLD_MODULE",
                    Description = "Old description"
                });


        var request = new UpdateModuleRequest
        {
            Name = "Updated Module",
            Code = "UPDATED_MODULE",
            Description = "Updated description"
        };


        // Act

        var response = await _client.PutAsJsonAsync(
            $"/api/Modules/{module.Id}",
            request);


        // Assert

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    // ============================================================
    // UPDATE - DOES NOT EXIST
    // ============================================================

    [Fact]
    public async Task Update_WhenModuleDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange

        var request = new UpdateModuleRequest
        {
            Name = "Updated Module",
            Code = "UPDATED_MODULE",
            Description = "Updated description"
        };


        // Act

        var response = await _client.PutAsJsonAsync(
            $"/api/Modules/{Guid.NewGuid()}",
            request);


        // Assert

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    // ============================================================
    // DELETE / DISABLE - EXISTS
    // ============================================================

    [Fact]
    public async Task Delete_WhenModuleExists_ShouldReturn204NoContent()
    {
        // Arrange

        var module =
            await _moduleService.CreateAsync(
                new CreateModuleRequest
                {
                    Name = "Disable Module",
                    Code = "DISABLE_MODULE",
                    Description = "Module to disable"
                });


        // Act

        var response = await _client.DeleteAsync(
            $"/api/Modules/{module.Id}");


        // Assert

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }


    // ============================================================
    // DELETE / DISABLE - DOES NOT EXIST
    // ============================================================

    [Fact]
    public async Task Delete_WhenModuleDoesNotExist_ShouldReturn404NotFound()
    {
        // Act

        var response = await _client.DeleteAsync(
            $"/api/Modules/{Guid.NewGuid()}");


        // Assert

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}