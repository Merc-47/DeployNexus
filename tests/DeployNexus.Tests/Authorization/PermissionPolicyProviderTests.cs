using DeployNexus.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;

namespace DeployNexus.Tests.Authorization;

public class PermissionPolicyProviderTests
{
    [Fact]
    public async Task GetPolicyAsync_WithPermissionPolicy_ShouldCreatePermissionRequirement()
    {
        // Arrange
        var options = new AuthorizationOptions();

        var provider =
            new PermissionPolicyProvider(
                Options.Create(options));

        // Act
        var policy =
            await provider.GetPolicyAsync(
                "Permission:ROLE_CREATE");

        // Assert
        Assert.NotNull(policy);

        var requirement =
            Assert.Single(policy.Requirements);

        var permissionRequirement =
            Assert.IsType<PermissionRequirement>(
                requirement);

        Assert.Equal(
            "ROLE_CREATE",
            permissionRequirement.PermissionCode);
    }


    [Fact]
    public async Task GetPolicyAsync_WithDifferentPermission_ShouldCreateCorrectRequirement()
    {
        // Arrange
        var options = new AuthorizationOptions();

        var provider =
            new PermissionPolicyProvider(
                Options.Create(options));

        // Act
        var policy =
            await provider.GetPolicyAsync(
                "Permission:USER_DELETE");

        // Assert
        Assert.NotNull(policy);

        var requirement =
            Assert.Single(policy.Requirements);

        var permissionRequirement =
            Assert.IsType<PermissionRequirement>(
                requirement);

        Assert.Equal(
            "USER_DELETE",
            permissionRequirement.PermissionCode);
    }


    [Fact]
    public async Task GetPolicyAsync_WithNormalPolicy_ShouldUseDefaultProvider()
    {
        // Arrange
        var options = new AuthorizationOptions();

        options.AddPolicy(
            "TestPolicy",
            policy =>
            {
                policy.RequireAuthenticatedUser();
            });

        var provider =
            new PermissionPolicyProvider(
                Options.Create(options));

        // Act
        var policy =
            await provider.GetPolicyAsync(
                "TestPolicy");

        // Assert
        Assert.NotNull(policy);

        Assert.Contains(
            policy.Requirements,
            requirement =>
                requirement is DenyAnonymousAuthorizationRequirement);
    }
}