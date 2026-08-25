using DeployNexus.Application.Authentication.DTOs;
using DeployNexus.Application.Authentication.Services;
using DeployNexus.Tests.Authentication;
using DeployNexus.Tests.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DeployNexus.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange

        var userRepository =
            new FakeUserRepository();

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
            new FakePasswordHasher();

        var jwtTokenGenerator =
            new FakeJwtTokenGenerator();

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
            Password = "TestPassword123!"
        };


        // Act

        var result =
            await controller.Login(request);


        // Assert

        var okResult =
            Assert.IsType<OkObjectResult>(
                result.Result);

        var response =
            Assert.IsType<LoginResponse>(
                okResult.Value);

        Assert.NotNull(response.Token);

        Assert.Equal(
            $"fake-token-{user.Id}",
            response.Token);

        Assert.True(
            response.ExpiresAt > DateTime.UtcNow);
    }


    [Fact]
    public async Task Login_WithInvalidUsername_ThrowsUnauthorizedAccessException()
    {
        // Arrange

        var userRepository =
            new FakeUserRepository();

        var passwordHasher =
            new FakePasswordHasher();

        var jwtTokenGenerator =
            new FakeJwtTokenGenerator();

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


        // Act & Assert

        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => controller.Login(request));


        Assert.Equal(
            "Invalid username or password",
            exception.Message);
    }


    [Fact]
    public async Task Login_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange

        var userRepository =
            new FakeUserRepository();

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
            new FakePasswordHasher();

        var jwtTokenGenerator =
            new FakeJwtTokenGenerator();

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


        // Act & Assert

        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => controller.Login(request));


        Assert.Equal(
            "Invalid username or password",
            exception.Message);
    }


    [Fact]
    public async Task Login_WithInactiveUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange

        var userRepository =
            new FakeUserRepository();

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
            new FakePasswordHasher();

        var jwtTokenGenerator =
            new FakeJwtTokenGenerator();

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


        // Act & Assert

        var exception =
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => controller.Login(request));


        Assert.Equal(
            "User account is inactive",
            exception.Message);
    }
}