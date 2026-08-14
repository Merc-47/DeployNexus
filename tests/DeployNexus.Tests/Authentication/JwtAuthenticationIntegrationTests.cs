using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DeployNexus.Application.Authentication.DTOs;
using DeployNexus.Application.Authentication.Interfaces;
using DeployNexus.Domain.Entities;
using DeployNexus.Tests.Integration;
using DeployNexus.Tests.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Tests.Authentication;

public class JwtAuthenticationIntegrationTests
    : IClassFixture<DeployNexusWebApplicationFactory>
{
    private readonly DeployNexusWebApplicationFactory _factory;

    public JwtAuthenticationIntegrationTests(
        DeployNexusWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userRepository =
            _factory.Services
                .GetRequiredService<FakeUserRepository>();

        var passwordHasher =
            _factory.Services
                .GetRequiredService<IPasswordHasher>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "jwt.test",
            Email = "jwt.test@deploynexus.local",
            PasswordHash = passwordHasher.Hash("TestPassword123!"),
            FirstName = "JWT",
            LastName = "Test",
            IsActive = true,
            OrganizationId = Guid.NewGuid()
        };

        await userRepository.AddAsync(user);

        var request = new LoginRequest
        {
            Username = "jwt.test",
            Password = "TestPassword123!"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(result);
        Assert.False(
            string.IsNullOrWhiteSpace(result.Token));

        Assert.True(
            result.ExpiresAt > DateTime.UtcNow);
    }


    [Fact]
    public async Task Me_WithoutToken_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response =
            await client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }


    [Fact]
    public async Task Me_WithInvalidToken_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                "this-is-not-a-valid-jwt");

        // Act
        var response =
            await client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }


    [Fact]
    public async Task Login_GeneratedToken_ContainsExpectedClaims()
    {
        // Arrange
        var client = _factory.CreateClient();

        var userRepository =
            _factory.Services
                .GetRequiredService<FakeUserRepository>();

        var passwordHasher =
            _factory.Services
                .GetRequiredService<IPasswordHasher>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "claims.test",
            Email = "claims.test@deploynexus.local",

            PasswordHash =
                passwordHasher.Hash("TestPassword123!"),

            FirstName = "Claims",
            LastName = "Test",
            IsActive = true,
            OrganizationId = Guid.NewGuid(),
            RoleId = Guid.NewGuid()
        };

        await userRepository.AddAsync(user);

        var request = new LoginRequest
        {
            Username = user.Username,
            Password = "TestPassword123!"
        };

        // Act
        var response =
            await client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(result);

        Assert.False(
            string.IsNullOrWhiteSpace(result.Token));

        var handler =
            new JwtSecurityTokenHandler();

        var token =
            handler.ReadJwtToken(result.Token);


        var nameIdentifier =
            token.Claims.FirstOrDefault(
                x =>
                    x.Type ==
                    System.Security.Claims.ClaimTypes.NameIdentifier);

        var name =
            token.Claims.FirstOrDefault(
                x =>
                    x.Type ==
                    System.Security.Claims.ClaimTypes.Name);

        var email =
            token.Claims.FirstOrDefault(
                x =>
                    x.Type ==
                    System.Security.Claims.ClaimTypes.Email);

        var roleId =
            token.Claims.FirstOrDefault(
                x =>
                    x.Type == "roleId");


        Assert.NotNull(nameIdentifier);
        Assert.NotNull(name);
        Assert.NotNull(email);
        Assert.NotNull(roleId);


        Assert.Equal(
            user.Id.ToString(),
            nameIdentifier!.Value);

        Assert.Equal(
            user.Username,
            name!.Value);

        Assert.Equal(
            user.Email,
            email!.Value);

        Assert.Equal(
            user.RoleId.ToString(),
            roleId!.Value);
    }
}