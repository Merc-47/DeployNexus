using System.Net;
using System.Net.Http.Json;
using DeployNexus.Application.Authentication.DTOs;
using DeployNexus.Application.Authentication.Interfaces;
using DeployNexus.Application.Authentication.Services;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Tests.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange

        var userRepository = new FakeUserRepository();

        var user = new DeployNexus.Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Username = "auth.test",
            Email = "auth.test@deploynexus.local",
            PasswordHash = "HASHED_TestPassword123!",
            FirstName = "Auth",
            LastName = "Test",
            IsActive = true,
            OrganizationId = Guid.NewGuid()
        };

        await userRepository.AddAsync(user);

        var passwordHasher = new DeployNexus.Tests.Authentication.FakePasswordHasher();

        var jwtTokenGenerator =
            new DeployNexus.Tests.Authentication.FakeJwtTokenGenerator();

        var authService = new AuthService(
            userRepository,
            passwordHasher,
            jwtTokenGenerator);

        var controller = new DeployNexus.API.Controllers.AuthController(
            authService);

        var request = new LoginRequest
        {
            Username = "auth.test",
            Password = "TestPassword123!"
        };

        // Act

        var result = await controller.Login(request);

        // Assert

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<LoginResponse>(okResult.Value);

        Assert.NotNull(response.Token);
        Assert.Equal(
            $"fake-token-{user.Id}",
            response.Token);

        Assert.True(
            response.ExpiresAt > DateTime.UtcNow);
    }


    [Fact]
    public async Task Login_WithInvalidUsername_ReturnsUnauthorized()
    {
        // Arrange

        var userRepository = new FakeUserRepository();

        var passwordHasher =
            new DeployNexus.Tests.Authentication.FakePasswordHasher();

        var jwtTokenGenerator =
            new DeployNexus.Tests.Authentication.FakeJwtTokenGenerator();

        var authService = new AuthService(
            userRepository,
            passwordHasher,
            jwtTokenGenerator);

        var controller =
            new DeployNexus.API.Controllers.AuthController(
                authService);

        var request = new LoginRequest
        {
            Username = "does.not.exist",
            Password = "TestPassword123!"
        };

        // Act

        var result = await controller.Login(request);

        // Assert

        var unauthorizedResult =
            Assert.IsType<UnauthorizedObjectResult>(
                result.Result);

        Assert.Equal(
            StatusCodes.Status401Unauthorized,
            unauthorizedResult.StatusCode);
    }


    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange

        var userRepository = new FakeUserRepository();

        var user = new DeployNexus.Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Username = "auth.test",
            Email = "auth.test@deploynexus.local",
            PasswordHash = "HASHED_TestPassword123!",
            FirstName = "Auth",
            LastName = "Test",
            IsActive = true,
            OrganizationId = Guid.NewGuid()
        };

        await userRepository.AddAsync(user);

        var passwordHasher =
            new DeployNexus.Tests.Authentication.FakePasswordHasher();

        var jwtTokenGenerator =
            new DeployNexus.Tests.Authentication.FakeJwtTokenGenerator();

        var authService = new AuthService(
            userRepository,
            passwordHasher,
            jwtTokenGenerator);

        var controller =
            new DeployNexus.API.Controllers.AuthController(
                authService);

        var request = new LoginRequest
        {
            Username = "auth.test",
            Password = "WrongPassword!"
        };

        // Act

        var result = await controller.Login(request);

        // Assert

        var unauthorizedResult =
            Assert.IsType<UnauthorizedObjectResult>(
                result.Result);

        Assert.Equal(
            StatusCodes.Status401Unauthorized,
            unauthorizedResult.StatusCode);
    }


    [Fact]
    public async Task Login_WithInactiveUser_ReturnsUnauthorized()
    {
        // Arrange

        var userRepository = new FakeUserRepository();

        var user = new DeployNexus.Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Username = "inactive.user",
            Email = "inactive@deploynexus.local",
            PasswordHash = "HASHED_TestPassword123!",
            FirstName = "Inactive",
            LastName = "User",
            IsActive = false,
            OrganizationId = Guid.NewGuid()
        };

        await userRepository.AddAsync(user);

        var passwordHasher =
            new DeployNexus.Tests.Authentication.FakePasswordHasher();

        var jwtTokenGenerator =
            new DeployNexus.Tests.Authentication.FakeJwtTokenGenerator();

        var authService = new AuthService(
            userRepository,
            passwordHasher,
            jwtTokenGenerator);

        var controller =
            new DeployNexus.API.Controllers.AuthController(
                authService);

        var request = new LoginRequest
        {
            Username = "inactive.user",
            Password = "TestPassword123!"
        };

        // Act

        var result = await controller.Login(request);

        // Assert

        var unauthorizedResult =
            Assert.IsType<UnauthorizedObjectResult>(
                result.Result);

        Assert.Equal(
            StatusCodes.Status401Unauthorized,
            unauthorizedResult.StatusCode);
    }
}