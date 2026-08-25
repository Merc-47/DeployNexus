using DeployNexus.Application.Common.Exceptions;
using DeployNexus.Application.Permissions.DTOs;
using DeployNexus.Application.Permissions.Services;
using DeployNexus.Domain.Entities;
using DeployNexus.Domain.Enums;
using DeployNexus.Tests.Repositories;

namespace DeployNexus.Tests.Permissions;

public class PermissionServiceTests
{
    private static (
        FakePermissionRepository PermissionRepository,
        FakeModuleRepository ModuleRepository,
        PermissionService Service)
        CreateService()
    {
        var permissionRepository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        var service =
            new PermissionService(
                permissionRepository,
                moduleRepository);

        return (
            permissionRepository,
            moduleRepository,
            service);
    }


    private static async Task<Module> CreateModule(
        FakeModuleRepository moduleRepository,
        string name = "User Management",
        string code = "USER_MANAGEMENT",
        ModuleStatus status = ModuleStatus.Available)
    {
        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = code,
            Description = "User management module",
            Status = status
        };

        await moduleRepository.AddAsync(module);

        return module;
    }


    // ============================================================
    // CREATE
    // ============================================================

    [Fact]
    public async Task CreateAsync_ShouldCreatePermissionSuccessfully()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();

        var module =
            await CreateModule(
                moduleRepository);

        var request =
            new CreatePermissionRequest
            {
                Name = "Create User",
                Code = "USER_CREATE",
                ModuleId = module.Id
            };


        // Act
        var result =
            await service.CreateAsync(request);


        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            "Create User",
            result.Name);

        Assert.Equal(
            "USER_CREATE",
            result.Code);

        Assert.Equal(
            module.Id,
            result.ModuleId);

        Assert.True(
            result.IsActive);
    }


    [Fact]
    public async Task CreateAsync_ShouldThrowNotFoundException_WhenModuleDoesNotExist()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();

        var request =
            new CreatePermissionRequest
            {
                Name = "Create User",
                Code = "USER_CREATE",
                ModuleId = Guid.NewGuid()
            };


        // Act
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(
                () => service.CreateAsync(request));


        // Assert
        Assert.Equal(
            "Module not found",
            exception.Message);
    }


    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenModuleIsDisabled()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();

        var module =
            await CreateModule(
                moduleRepository,
                "Disabled Module",
                "DISABLED",
                ModuleStatus.Disabled);


        var request =
            new CreatePermissionRequest
            {
                Name = "Create User",
                Code = "USER_CREATE",
                ModuleId = module.Id
            };


        // Act
        var exception =
            await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(request));


        // Assert
        Assert.Equal(
            "Cannot create permission for a disabled module",
            exception.Message);
    }


    // ============================================================
    // GET BY ID
    // ============================================================

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPermission_WhenExists()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();

        var module =
            await CreateModule(
                moduleRepository);


        var created =
            await service.CreateAsync(
                new CreatePermissionRequest
                {
                    Name = "Delete User",
                    Code = "USER_DELETE",
                    ModuleId = module.Id
                });


        // Act
        var result =
            await service.GetByIdAsync(
                created.Id);


        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            created.Id,
            result.Id);

        Assert.Equal(
            module.Id,
            result.ModuleId);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenPermissionDoesNotExist()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();


        // Act
        var result =
            await service.GetByIdAsync(
                Guid.NewGuid());


        // Assert
        Assert.Null(result);
    }


    // ============================================================
    // GET ALL
    // ============================================================

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPermissions()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();

        var module =
            await CreateModule(
                moduleRepository);


        await service.CreateAsync(
            new CreatePermissionRequest
            {
                Name = "Create User",
                Code = "USER_CREATE",
                ModuleId = module.Id
            });


        await service.CreateAsync(
            new CreatePermissionRequest
            {
                Name = "Delete User",
                Code = "USER_DELETE",
                ModuleId = module.Id
            });


        // Act
        var result =
            await service.GetAllAsync();


        // Assert
        Assert.Equal(
            2,
            result.Count());
    }


    // ============================================================
    // UPDATE
    // ============================================================

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePermission_WhenExists()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();

        var module =
            await CreateModule(
                moduleRepository);


        var created =
            await service.CreateAsync(
                new CreatePermissionRequest
                {
                    Name = "Old Name",
                    Code = "OLD",
                    ModuleId = module.Id
                });


        var updateRequest =
            new UpdatePermissionRequest
            {
                Name = "Updated Name",
                Code = "UPDATED"
            };


        // Act
        var result =
            await service.UpdateAsync(
                created.Id,
                updateRequest);


        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            "Updated Name",
            result.Name);

        Assert.Equal(
            "UPDATED",
            result.Code);

        Assert.Equal(
            module.Id,
            result.ModuleId);
    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenPermissionDoesNotExist()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();


        var request =
            new UpdatePermissionRequest
            {
                Name = "Updated Name",
                Code = "UPDATED"
            };


        // Act
        var result =
            await service.UpdateAsync(
                Guid.NewGuid(),
                request);


        // Assert
        Assert.Null(result);
    }


    // ============================================================
    // DEACTIVATE
    // ============================================================

    [Fact]
    public async Task DeactivateAsync_ShouldDeactivatePermission_WhenExists()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();

        var module =
            await CreateModule(
                moduleRepository);


        var created =
            await service.CreateAsync(
                new CreatePermissionRequest
                {
                    Name = "Create User",
                    Code = "USER_CREATE",
                    ModuleId = module.Id
                });


        // Act
        var result =
            await service.DeactivateAsync(
                created.Id);


        var permission =
            await service.GetByIdAsync(
                created.Id);


        // Assert
        Assert.True(
            result.Success);

        Assert.NotNull(
            permission);

        Assert.False(
            permission.IsActive);
    }


    [Fact]
    public async Task DeactivateAsync_ShouldFail_WhenPermissionDoesNotExist()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();


        // Act
        var result =
            await service.DeactivateAsync(
                Guid.NewGuid());


        // Assert
        Assert.False(
            result.Success);

        Assert.Equal(
            "Permission not found",
            result.Message);
    }


    // ============================================================
    // HAS PERMISSION
    // ============================================================

    [Fact]
    public async Task HasPermissionAsync_WhenUserHasPermission_ReturnsTrue()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();


        var userId =
            Guid.NewGuid();


        permissionRepository.SetUserPermission(
            userId,
            "USER_CREATE",
            true);


        // Act
        var result =
            await service.HasPermissionAsync(
                userId,
                "USER_CREATE");


        // Assert
        Assert.True(
            result);
    }


    [Fact]
    public async Task HasPermissionAsync_WhenUserDoesNotHavePermission_ReturnsFalse()
    {
        // Arrange
        var (
            permissionRepository,
            moduleRepository,
            service) = CreateService();


        var userId =
            Guid.NewGuid();


        permissionRepository.SetUserPermission(
            userId,
            "USER_CREATE",
            false);


        // Act
        var result =
            await service.HasPermissionAsync(
                userId,
                "USER_CREATE");


        // Assert
        Assert.False(
            result);
    }
}