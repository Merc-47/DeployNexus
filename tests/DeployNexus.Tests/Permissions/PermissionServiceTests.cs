using DeployNexus.Application.Permissions.DTOs;
using DeployNexus.Application.Permissions.Services;
using DeployNexus.Tests.Repositories;

namespace DeployNexus.Tests.Permissions;

public class PermissionServiceTests
{

    [Fact]
    public async Task CreateAsync_ShouldCreatePermissionSuccessfully()
    {
        // Arrange
        var repository = new FakePermissionRepository();

        var service = new PermissionService(repository);


        var request = new CreatePermissionRequest
        {
            Name = "Create User",
            Code = "USER_CREATE"
        };


        // Act
        var result = await service.CreateAsync(request);


        // Assert
        Assert.NotNull(result);
        Assert.Equal("Create User", result.Name);
        Assert.Equal("USER_CREATE", result.Code);
        Assert.True(result.IsActive);
    }



    [Fact]
    public async Task GetByIdAsync_ShouldReturnPermission_WhenExists()
    {
        // Arrange
        var repository = new FakePermissionRepository();

        var service = new PermissionService(repository);


        var created = await service.CreateAsync(
            new CreatePermissionRequest
            {
                Name = "Delete User",
                Code = "USER_DELETE"
            });


        // Act
        var result =
            await service.GetByIdAsync(created.Id);


        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }




    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPermissions()
    {
        // Arrange
        var repository = new FakePermissionRepository();

        var service = new PermissionService(repository);


        await service.CreateAsync(
            new CreatePermissionRequest
            {
                Name = "Create User",
                Code = "USER_CREATE"
            });


        await service.CreateAsync(
            new CreatePermissionRequest
            {
                Name = "Delete User",
                Code = "USER_DELETE"
            });


        // Act
        var result = await service.GetAllAsync();


        // Assert
        Assert.Equal(2, result.Count());
    }




    [Fact]
    public async Task UpdateAsync_ShouldUpdatePermission_WhenExists()
    {
        // Arrange
        var repository = new FakePermissionRepository();

        var service = new PermissionService(repository);


        var created = await service.CreateAsync(
            new CreatePermissionRequest
            {
                Name = "Old Name",
                Code = "OLD"
            });


        var updateRequest = new UpdatePermissionRequest
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
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("UPDATED", result.Code);
    }





    [Fact]
    public async Task DeactivateAsync_ShouldDeactivatePermission_WhenExists()
    {
        // Arrange
        var repository = new FakePermissionRepository();

        var service = new PermissionService(repository);


        var created = await service.CreateAsync(
            new CreatePermissionRequest
            {
                Name = "Create User",
                Code = "USER_CREATE"
            });


        // Act
        var result =
            await service.DeactivateAsync(created.Id);


        var permission =
            await service.GetByIdAsync(created.Id);


        // Assert
        Assert.True(result.Success);
        Assert.NotNull(permission);
        Assert.False(permission.IsActive);
    }
}