using DeployNexus.Application.Permissions.DTOs;
using DeployNexus.Application.Permissions.Services;
using DeployNexus.Domain.Entities;
using DeployNexus.Domain.Enums;
using DeployNexus.Tests.Repositories;

namespace DeployNexus.Tests.Permissions;

public class PermissionServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreatePermissionSuccessfully()
    {
        // Arrange
        var permissionRepository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = "User Management",
            Code = "USER_MANAGEMENT",
            Description = "User management module",
            Status = ModuleStatus.Available
        };

        await moduleRepository.AddAsync(module);

        var service = new PermissionService(
            permissionRepository,
            moduleRepository);

        var request = new CreatePermissionRequest
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

        Assert.True(result.IsActive);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnPermission_WhenExists()
    {
        // Arrange
        var permissionRepository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = "User Management",
            Code = "USER_MANAGEMENT",
            Description = "User management module",
            Status = ModuleStatus.Available
        };

        await moduleRepository.AddAsync(module);

        var service = new PermissionService(
            permissionRepository,
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
            await service.GetByIdAsync(created.Id);


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
    public async Task GetAllAsync_ShouldReturnAllPermissions()
    {
        // Arrange
        var permissionRepository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = "User Management",
            Code = "USER_MANAGEMENT",
            Description = "User management module",
            Status = ModuleStatus.Available
        };

        await moduleRepository.AddAsync(module);

        var service = new PermissionService(
            permissionRepository,
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


    [Fact]
    public async Task UpdateAsync_ShouldUpdatePermission_WhenExists()
    {
        // Arrange
        var permissionRepository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = "User Management",
            Code = "USER_MANAGEMENT",
            Description = "User management module",
            Status = ModuleStatus.Available
        };

        await moduleRepository.AddAsync(module);

        var service = new PermissionService(
            permissionRepository,
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
    public async Task DeactivateAsync_ShouldDeactivatePermission_WhenExists()
    {
        // Arrange
        var permissionRepository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = "User Management",
            Code = "USER_MANAGEMENT",
            Description = "User management module",
            Status = ModuleStatus.Available
        };

        await moduleRepository.AddAsync(module);

        var service = new PermissionService(
            permissionRepository,
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
        Assert.True(result.Success);

        Assert.NotNull(permission);

        Assert.False(
            permission.IsActive);
    }


    [Fact]
    public async Task HasPermissionAsync_WhenUserHasPermission_ReturnsTrue()
    {
        // Arrange
        var userId =
            Guid.NewGuid();

        var permissionRepository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();


        permissionRepository.SetUserPermission(
            userId,
            "USER_CREATE",
            true);


        var service = new PermissionService(
            permissionRepository,
            moduleRepository);


        // Act
        var result =
            await service.HasPermissionAsync(
                userId,
                "USER_CREATE");


        // Assert
        Assert.True(result);
    }


    [Fact]
    public async Task HasPermissionAsync_WhenUserDoesNotHavePermission_ReturnsFalse()
    {
        // Arrange
        var userId =
            Guid.NewGuid();

        var permissionRepository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();


        permissionRepository.SetUserPermission(
            userId,
            "USER_CREATE",
            false);


        var service = new PermissionService(
            permissionRepository,
            moduleRepository);


        // Act
        var result =
            await service.HasPermissionAsync(
                userId,
                "USER_CREATE");


        // Assert
        Assert.False(result);
    }


    [Fact]
    public async Task CreateAsync_ShouldFail_WhenModuleDoesNotExist()
    {
        // Arrange
        var permissionRepository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        var service = new PermissionService(
            permissionRepository,
            moduleRepository);


        var request =
            new CreatePermissionRequest
            {
                Name = "Create User",
                Code = "USER_CREATE",
                ModuleId = Guid.NewGuid()
            };


        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => service.CreateAsync(request));
    }


    [Fact]
    public async Task CreateAsync_ShouldFail_WhenModuleIsDisabled()
    {
        // Arrange
        var permissionRepository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        var module = new Module
        {
            Id = Guid.NewGuid(),
            Name = "Disabled Module",
            Code = "DISABLED",
            Description = "Disabled module",
            Status = ModuleStatus.Disabled
        };

        await moduleRepository.AddAsync(module);


        var service = new PermissionService(
            permissionRepository,
            moduleRepository);


        var request =
            new CreatePermissionRequest
            {
                Name = "Create User",
                Code = "USER_CREATE",
                ModuleId = module.Id
            };


        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => service.CreateAsync(request));
    }
}