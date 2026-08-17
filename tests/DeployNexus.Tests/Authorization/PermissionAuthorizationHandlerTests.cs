using System.Security.Claims;
using DeployNexus.API.Authorization;
using DeployNexus.Application.Permissions.Services;
using DeployNexus.Tests.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace DeployNexus.Tests.Authorization;

public class PermissionAuthorizationHandlerTests
{
    [Fact]
    public async Task HandleRequirementAsync_WhenUserHasPermission_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var repository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        repository.SetUserPermission(
            userId,
            "ROLE_VIEW",
            true);

        var permissionService =
            new PermissionService(
                repository,
                moduleRepository);

        var handler =
            new PermissionAuthorizationHandler(
                permissionService);

        var user =
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(
                            ClaimTypes.NameIdentifier,
                            userId.ToString())
                    },
                    "TestAuth"));

        var requirement =
            new PermissionRequirement(
                "ROLE_VIEW");

        var context =
            new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);


        // Act
        await handler.HandleAsync(context);


        // Assert
        Assert.True(
            context.HasSucceeded);
    }


    [Fact]
    public async Task HandleRequirementAsync_WhenUserDoesNotHavePermission_ShouldNotSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var repository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        repository.SetUserPermission(
            userId,
            "ROLE_DELETE",
            false);

        var permissionService =
            new PermissionService(
                repository,
                moduleRepository);

        var handler =
            new PermissionAuthorizationHandler(
                permissionService);

        var user =
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(
                            ClaimTypes.NameIdentifier,
                            userId.ToString())
                    },
                    "TestAuth"));

        var requirement =
            new PermissionRequirement(
                "ROLE_DELETE");

        var context =
            new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);


        // Act
        await handler.HandleAsync(context);


        // Assert
        Assert.False(
            context.HasSucceeded);
    }


    [Fact]
    public async Task HandleRequirementAsync_WhenNameIdentifierClaimIsMissing_ShouldNotSucceed()
    {
        // Arrange
        var repository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        var permissionService =
            new PermissionService(
                repository,
                moduleRepository);

        var handler =
            new PermissionAuthorizationHandler(
                permissionService);

        var user =
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    Array.Empty<Claim>(),
                    "TestAuth"));

        var requirement =
            new PermissionRequirement(
                "ROLE_VIEW");

        var context =
            new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);


        // Act
        await handler.HandleAsync(context);


        // Assert
        Assert.False(
            context.HasSucceeded);
    }


    [Fact]
    public async Task HandleRequirementAsync_WhenUserIdClaimIsInvalidGuid_ShouldNotSucceed()
    {
        // Arrange
        var repository =
            new FakePermissionRepository();

        var moduleRepository =
            new FakeModuleRepository();

        var permissionService =
            new PermissionService(
                repository,
                moduleRepository);

        var handler =
            new PermissionAuthorizationHandler(
                permissionService);

        var user =
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(
                            ClaimTypes.NameIdentifier,
                            "not-a-guid")
                    },
                    "TestAuth"));

        var requirement =
            new PermissionRequirement(
                "ROLE_VIEW");

        var context =
            new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);


        // Act
        await handler.HandleAsync(context);


        // Assert
        Assert.False(
            context.HasSucceeded);
    }
}