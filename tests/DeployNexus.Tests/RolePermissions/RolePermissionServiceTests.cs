using DeployNexus.Application.RolePermissions.DTOs;
using DeployNexus.Application.RolePermissions.Services;
using DeployNexus.Domain.Entities;
using DeployNexus.Tests.Repositories;
using DeployNexus.Tests.Roles;

namespace DeployNexus.Tests.RolePermissions;

public class RolePermissionServiceTests
{
    [Fact]
    public async Task AssignAsync_ShouldAssignPermissionSuccessfully()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var permissionRepository = new FakePermissionRepository();
        var rolePermissionRepository =
            new FakeRolePermissionRepository();

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            IsActive = true
        };

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "CREATE_USER",
            Code = "USER_CREATE",
            IsActive = true
        };

        await roleRepository.AddAsync(role);
        await permissionRepository.AddAsync(permission);

        var service = new RolePermissionService(
            rolePermissionRepository,
            roleRepository,
            permissionRepository);

        var request = new AssignPermissionRequest
        {
            PermissionId = permission.Id
        };

        // Act
        var result = await service.AssignAsync(
            role.Id,
            request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(role.Id, result.RoleId);
        Assert.Equal(permission.Id, result.PermissionId);
        Assert.Equal("CREATE_USER", result.PermissionName);
        Assert.Equal("USER_CREATE", result.PermissionCode);

        var assignment =
            await rolePermissionRepository.GetAsync(
                role.Id,
                permission.Id);

        Assert.NotNull(assignment);
    }


    [Fact]
    public async Task AssignAsync_ShouldFail_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var permissionRepository = new FakePermissionRepository();
        var rolePermissionRepository =
            new FakeRolePermissionRepository();

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "CREATE_USER",
            Code = "USER_CREATE",
            IsActive = true
        };

        await permissionRepository.AddAsync(permission);

        var service = new RolePermissionService(
            rolePermissionRepository,
            roleRepository,
            permissionRepository);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            service.AssignAsync(
                Guid.NewGuid(),
                new AssignPermissionRequest
                {
                    PermissionId = permission.Id
                }));
    }


    [Fact]
    public async Task AssignAsync_ShouldFail_WhenPermissionDoesNotExist()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var permissionRepository = new FakePermissionRepository();
        var rolePermissionRepository =
            new FakeRolePermissionRepository();

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            IsActive = true
        };

        await roleRepository.AddAsync(role);

        var service = new RolePermissionService(
            rolePermissionRepository,
            roleRepository,
            permissionRepository);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            service.AssignAsync(
                role.Id,
                new AssignPermissionRequest
                {
                    PermissionId = Guid.NewGuid()
                }));
    }


    [Fact]
    public async Task AssignAsync_ShouldFail_WhenPermissionAlreadyAssigned()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var permissionRepository = new FakePermissionRepository();
        var rolePermissionRepository =
            new FakeRolePermissionRepository();

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            IsActive = true
        };

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "CREATE_USER",
            Code = "USER_CREATE",
            IsActive = true
        };

        await roleRepository.AddAsync(role);
        await permissionRepository.AddAsync(permission);

        await rolePermissionRepository.AddAsync(
            new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                PermissionId = permission.Id,
                Permission = permission
            });

        var service = new RolePermissionService(
            rolePermissionRepository,
            roleRepository,
            permissionRepository);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            service.AssignAsync(
                role.Id,
                new AssignPermissionRequest
                {
                    PermissionId = permission.Id
                }));
    }


    [Fact]
    public async Task RemoveAsync_ShouldRemovePermission()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var permissionRepository = new FakePermissionRepository();
        var rolePermissionRepository =
            new FakeRolePermissionRepository();

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            IsActive = true
        };

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "CREATE_USER",
            Code = "USER_CREATE",
            IsActive = true
        };

        await roleRepository.AddAsync(role);
        await permissionRepository.AddAsync(permission);

        await rolePermissionRepository.AddAsync(
            new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                PermissionId = permission.Id,
                Permission = permission
            });

        var service = new RolePermissionService(
            rolePermissionRepository,
            roleRepository,
            permissionRepository);

        // Act
        var result = await service.RemoveAsync(
            role.Id,
            permission.Id);

        // Assert
        Assert.True(result);

        var assignment =
            await rolePermissionRepository.GetAsync(
                role.Id,
                permission.Id);

        Assert.Null(assignment);
    }


    [Fact]
    public async Task RemoveAsync_ShouldReturnFalse_WhenAssignmentDoesNotExist()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var permissionRepository = new FakePermissionRepository();
        var rolePermissionRepository =
            new FakeRolePermissionRepository();

        var service = new RolePermissionService(
            rolePermissionRepository,
            roleRepository,
            permissionRepository);

        // Act
        var result = await service.RemoveAsync(
            Guid.NewGuid(),
            Guid.NewGuid());

        // Assert
        Assert.False(result);
    }


    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnPermissions()
    {
        // Arrange
        var roleRepository = new FakeRoleRepository();
        var permissionRepository = new FakePermissionRepository();
        var rolePermissionRepository =
            new FakeRolePermissionRepository();

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            IsActive = true
        };

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "CREATE_USER",
            Code = "USER_CREATE",
            IsActive = true
        };

        await roleRepository.AddAsync(role);
        await permissionRepository.AddAsync(permission);

        await rolePermissionRepository.AddAsync(
            new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                PermissionId = permission.Id,
                Permission = permission
            });

        var service = new RolePermissionService(
            rolePermissionRepository,
            roleRepository,
            permissionRepository);

        // Act
        var result = await service.GetPermissionsAsync(
            role.Id);

        // Assert
        var permissions = result.ToList();

        Assert.Single(permissions);

        var dto = permissions.First();

        Assert.Equal(role.Id, dto.RoleId);
        Assert.Equal(permission.Id, dto.PermissionId);
        Assert.Equal("CREATE_USER", dto.PermissionName);
        Assert.Equal("USER_CREATE", dto.PermissionCode);
    }
}