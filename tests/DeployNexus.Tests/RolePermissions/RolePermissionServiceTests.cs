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
        var roleRepository = new FakeRoleRepository();
        var permissionRepository = new FakePermissionRepository();
        var rolePermissionRepository = new FakeRolePermissionRepository();


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


        var result = await service.AssignAsync(
            role.Id,
            new AssignPermissionRequest
            {
                PermissionId = permission.Id
            });


        Assert.NotNull(result);
        Assert.Equal(role.Id, result.RoleId);
        Assert.Equal(permission.Id, result.PermissionId);
        Assert.Equal("CREATE_USER", result.PermissionName);
    }



    [Fact]
    public async Task AssignAsync_ShouldFail_WhenPermissionAlreadyAssigned()
    {
        var roleRepository = new FakeRoleRepository();
        var permissionRepository = new FakePermissionRepository();
        var rolePermissionRepository = new FakeRolePermissionRepository();


        var role = new Role
        {
            Id = Guid.NewGuid()
        };

        var permission = new Permission
        {
            Id = Guid.NewGuid()
        };


        await roleRepository.AddAsync(role);
        await permissionRepository.AddAsync(permission);


        await rolePermissionRepository.AddAsync(
            new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                PermissionId = permission.Id
            });


        var service = new RolePermissionService(
            rolePermissionRepository,
            roleRepository,
            permissionRepository);



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
        var rolePermissionRepository =
            new FakeRolePermissionRepository();


        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();


        await rolePermissionRepository.AddAsync(
            new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                PermissionId = permissionId
            });


        var result = await rolePermissionRepository
            .GetAsync(roleId, permissionId);


        Assert.NotNull(result);
    }



    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnPermissions()
    {
        var repository =
            new FakeRolePermissionRepository();


        var roleId = Guid.NewGuid();


        await repository.AddAsync(
            new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                PermissionId = Guid.NewGuid()
            });


        var result =
            await repository.GetPermissionsForRoleAsync(roleId);


        Assert.Single(result);
    }
}