using DeployNexus.Application.Common.Exceptions;
using DeployNexus.Application.Modules.DTOs;
using DeployNexus.Application.Modules.Services;
using DeployNexus.Domain.Enums;
using DeployNexus.Tests.Repositories;

namespace DeployNexus.Tests.Modules;

public class ModuleServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateModuleSuccessfully()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);

        var request =
            new CreateModuleRequest
            {
                Name = "User Management",
                Code = "USER_MANAGEMENT",
                Description = "User management module"
            };


        // Act
        var result =
            await service.CreateAsync(request);


        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            "User Management",
            result.Name);

        Assert.Equal(
            "USER_MANAGEMENT",
            result.Code);

        Assert.Equal(
            "User management module",
            result.Description);

        Assert.Equal(
            ModuleStatus.Available,
            result.Status);
    }


    [Fact]
    public async Task CreateAsync_ShouldFail_WhenCodeAlreadyExists()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);

        var request =
            new CreateModuleRequest
            {
                Name = "User Management",
                Code = "USER_MANAGEMENT",
                Description = "User management module"
            };


        await service.CreateAsync(request);


        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateAsync(request));
    }


    [Fact]
    public async Task CreateAsync_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);

        var request =
            new CreateModuleRequest
            {
                Name = "",
                Code = "USER_MANAGEMENT",
                Description = "User management module"
            };


        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(request));
    }


    [Fact]
    public async Task CreateAsync_ShouldFail_WhenCodeIsEmpty()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);

        var request =
            new CreateModuleRequest
            {
                Name = "User Management",
                Code = "",
                Description = "User management module"
            };


        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(request));
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnModule_WhenExists()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);

        var created =
            await service.CreateAsync(
                new CreateModuleRequest
                {
                    Name = "Role Management",
                    Code = "ROLE_MANAGEMENT",
                    Description = "Role management module"
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
            "ROLE_MANAGEMENT",
            result.Code);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);


        // Act
        var result =
            await service.GetByIdAsync(
                Guid.NewGuid());


        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task GetAllAsync_ShouldReturnAllModules()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);


        await service.CreateAsync(
            new CreateModuleRequest
            {
                Name = "User Management",
                Code = "USER_MANAGEMENT",
                Description = "Users"
            });


        await service.CreateAsync(
            new CreateModuleRequest
            {
                Name = "Role Management",
                Code = "ROLE_MANAGEMENT",
                Description = "Roles"
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
    public async Task UpdateAsync_ShouldUpdateModule_WhenExists()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);


        var created =
            await service.CreateAsync(
                new CreateModuleRequest
                {
                    Name = "Old Name",
                    Code = "OLD_CODE",
                    Description = "Old Description"
                });


        var request =
            new UpdateModuleRequest
            {
                Name = "Updated Name",
                Code = "UPDATED_CODE",
                Description = "Updated Description"
            };


        // Act
        var result =
            await service.UpdateAsync(
                created.Id,
                request);


        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            "Updated Name",
            result.Name);

        Assert.Equal(
            "UPDATED_CODE",
            result.Code);

        Assert.Equal(
            "Updated Description",
            result.Description);
    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenModuleDoesNotExist()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);

        var request =
            new UpdateModuleRequest
            {
                Name = "Updated",
                Code = "UPDATED",
                Description = "Updated"
            };


        // Act
        var result =
            await service.UpdateAsync(
                Guid.NewGuid(),
                request);


        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenNewCodeAlreadyExists()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);


        var first =
            await service.CreateAsync(
                new CreateModuleRequest
                {
                    Name = "User Management",
                    Code = "USER_MANAGEMENT",
                    Description = "Users"
                });


        await service.CreateAsync(
            new CreateModuleRequest
            {
                Name = "Role Management",
                Code = "ROLE_MANAGEMENT",
                Description = "Roles"
            });


        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(
            () => service.UpdateAsync(
                first.Id,
                new UpdateModuleRequest
                {
                    Name = "User Management",
                    Code = "ROLE_MANAGEMENT",
                    Description = "Updated"
                }));
    }


    [Fact]
    public async Task DisableAsync_ShouldDisableModule_WhenExists()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);


        var created =
            await service.CreateAsync(
                new CreateModuleRequest
                {
                    Name = "Deployment",
                    Code = "DEPLOYMENT",
                    Description = "Deployment module"
                });


        // Act
        var result =
            await service.DisableAsync(
                created.Id);


        var module =
            await service.GetByIdAsync(
                created.Id);


        // Assert
        Assert.True(result.Success);

        Assert.NotNull(module);

        Assert.Equal(
            ModuleStatus.Disabled,
            module.Status);
    }


    [Fact]
    public async Task DisableAsync_ShouldFail_WhenModuleDoesNotExist()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);


        // Act
        var result =
            await service.DisableAsync(
                Guid.NewGuid());


        // Assert
        Assert.False(
            result.Success);

        Assert.Equal(
            "Module not found",
            result.Message);
    }


    [Fact]
    public async Task DisableAsync_ShouldFail_WhenModuleIsAlreadyDisabled()
    {
        // Arrange
        var repository =
            new FakeModuleRepository();

        var service =
            new ModuleService(repository);


        var created =
            await service.CreateAsync(
                new CreateModuleRequest
                {
                    Name = "Deployment",
                    Code = "DEPLOYMENT",
                    Description = "Deployment module"
                });


        await service.DisableAsync(
            created.Id);


        // Act
        var result =
            await service.DisableAsync(
                created.Id);


        // Assert
        Assert.False(
            result.Success);

        Assert.Equal(
            "Module is already disabled",
            result.Message);
    }
}