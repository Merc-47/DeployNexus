using DeployNexus.API.Controllers;
using DeployNexus.Application.Common;
using DeployNexus.Application.Organizations.DTOs;
using DeployNexus.Application.Organizations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeployNexus.Tests.Controllers;

public class OrganizationsControllerTests
{
    private readonly FakeOrganizationService _service;
    private readonly OrganizationsController _controller;

    public OrganizationsControllerTests()
    {
        _service = new FakeOrganizationService();
        _controller = new OrganizationsController(_service);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateOrganizationRequest
        {
            Name = "Acme Corporation",
            Code = "ACME"
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult =
            Assert.IsType<CreatedAtActionResult>(result);

        var organization =
            Assert.IsType<OrganizationDto>(createdResult.Value);

        Assert.Equal("Acme Corporation", organization.Name);
        Assert.Equal("ACME", organization.Code);
        Assert.Equal(nameof(OrganizationsController.GetById),
            createdResult.ActionName);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange
        _service.Organizations.Add(
            new OrganizationDto
            {
                Id = Guid.NewGuid(),
                Name = "Org 1",
                Code = "ORG1",
                IsActive = true
            });

        _service.Organizations.Add(
            new OrganizationDto
            {
                Id = Guid.NewGuid(),
                Name = "Org 2",
                Code = "ORG2",
                IsActive = true
            });

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var organizations =
            Assert.IsAssignableFrom<IEnumerable<OrganizationDto>>(
                okResult.Value);

        Assert.Equal(2, organizations.Count());
    }

    [Fact]
    public async Task GetById_WhenOrganizationExists_ReturnsOk()
    {
        // Arrange
        var organization = new OrganizationDto
        {
            Id = Guid.NewGuid(),
            Name = "Acme",
            Code = "ACME",
            IsActive = true
        };

        _service.Organizations.Add(organization);

        // Act
        var result =
            await _controller.GetById(organization.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var returned =
            Assert.IsType<OrganizationDto>(okResult.Value);

        Assert.Equal(organization.Id, returned.Id);
    }

    [Fact]
    public async Task GetById_WhenOrganizationDoesNotExist_ReturnsNotFound()
    {
        // Act
        var result =
            await _controller.GetById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_WhenOrganizationExists_ReturnsOk()
    {
        // Arrange
        var organization = new OrganizationDto
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Code = "OLD",
            IsActive = true
        };

        _service.Organizations.Add(organization);

        var request = new UpdateOrganizationRequest
        {
            Name = "New Name",
            Code = "NEW"
        };

        // Act
        var result =
            await _controller.Update(organization.Id, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var updated =
            Assert.IsType<OrganizationDto>(okResult.Value);

        Assert.Equal("New Name", updated.Name);
        Assert.Equal("NEW", updated.Code);
    }

    [Fact]
    public async Task Update_WhenOrganizationDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateOrganizationRequest
        {
            Name = "New Name",
            Code = "NEW"
        };

        // Act
        var result =
            await _controller.Update(Guid.NewGuid(), request);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Deactivate_WhenOrganizationExists_ReturnsNoContent()
    {
        // Arrange
        var organization = new OrganizationDto
        {
            Id = Guid.NewGuid(),
            Name = "Acme",
            Code = "ACME",
            IsActive = true
        };

        _service.Organizations.Add(organization);

        // Act
        var result =
            await _controller.Deactivate(organization.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);

        Assert.False(
            _service.Organizations
                .First(x => x.Id == organization.Id)
                .IsActive);
    }

    [Fact]
    public async Task Deactivate_WhenOrganizationDoesNotExist_ReturnsNotFound()
    {
        // Act
        var result =
            await _controller.Deactivate(Guid.NewGuid());

        // Assert
        var notFound =
            Assert.IsType<NotFoundObjectResult>(result);

        Assert.Equal(
            "Organization not found",
            notFound.Value);
    }
}


// ============================================================
// Fake Organization Service
// ============================================================

public class FakeOrganizationService : IOrganizationService
{
    public List<OrganizationDto> Organizations { get; } = new();

    public Task<OrganizationDto> CreateAsync(
        CreateOrganizationRequest request)
    {
        var organization = new OrganizationDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            IsActive = true
        };

        Organizations.Add(organization);

        return Task.FromResult(organization);
    }

    public Task<OrganizationDto?> GetByIdAsync(Guid id)
    {
        var organization =
            Organizations.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(organization);
    }

    public Task<IEnumerable<OrganizationDto>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<OrganizationDto>>(
            Organizations);
    }

    public Task<OrganizationDto?> UpdateAsync(
        Guid id,
        UpdateOrganizationRequest request)
    {
        var organization =
            Organizations.FirstOrDefault(x => x.Id == id);

        if (organization == null)
            return Task.FromResult<OrganizationDto?>(null);

        organization.Name = request.Name;
        organization.Code = request.Code;

        return Task.FromResult<OrganizationDto?>(organization);
    }

    public Task<Result> DeactivateAsync(Guid id)
    {
        var organization =
            Organizations.FirstOrDefault(x => x.Id == id);

        if (organization == null)
        {
            return Task.FromResult(
                Result.Failure("Organization not found"));
        }

        organization.IsActive = false;

        return Task.FromResult(Result.Ok());
    }
}