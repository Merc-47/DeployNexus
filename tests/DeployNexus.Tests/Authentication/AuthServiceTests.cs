using DeployNexus.Application.Authentication.DTOs;
using DeployNexus.Application.Authentication.Services;
using DeployNexus.Domain.Entities;
using DeployNexus.Tests.Repositories;

namespace DeployNexus.Tests.Authentication;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var user = new User
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

        var userRepository = new FakeUserRepository();

        await userRepository.AddAsync(user);

        var passwordHasher = new FakePasswordHasher();
        var jwtTokenGenerator = new FakeJwtTokenGenerator();

        var service = new AuthService(
            userRepository,
            passwordHasher,
            jwtTokenGenerator);

        var request = new LoginRequest
        {
            Username = "auth.test",
            Password = "TestPassword123!"
        };

        var result = await service.LoginAsync(request);

        Assert.NotNull(result);
        Assert.Equal($"fake-token-{user.Id}", result.Token);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }
    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ThrowsUnauthorizedAccessException()
    {
        var userRepository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var jwtTokenGenerator = new FakeJwtTokenGenerator();

        var service = new AuthService(
            userRepository,
            passwordHasher,
            jwtTokenGenerator);

        var request = new LoginRequest
        {
            Username = "does.not.exist",
            Password = "TestPassword123!"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(request));
    }
    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "auth.test",
            Email = "auth.test@deploynexus.local",
            PasswordHash = "HASHED_TestPassword123!",
            IsActive = true,
            OrganizationId = Guid.NewGuid()
        };

        var userRepository = new FakeUserRepository();
        await userRepository.AddAsync(user);

        var passwordHasher = new FakePasswordHasher();
        var jwtTokenGenerator = new FakeJwtTokenGenerator();

        var service = new AuthService(
            userRepository,
            passwordHasher,
            jwtTokenGenerator);

        var request = new LoginRequest
        {
            Username = "auth.test",
            Password = "WrongPassword!"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(request));
    }
    [Fact]
    public async Task LoginAsync_WithInactiveUser_ThrowsUnauthorizedAccessException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "inactive.user",
            Email = "inactive@deploynexus.local",
            PasswordHash = "HASHED_TestPassword123!",
            IsActive = false,
            OrganizationId = Guid.NewGuid()
        };

        var userRepository = new FakeUserRepository();
        await userRepository.AddAsync(user);

        var passwordHasher = new FakePasswordHasher();
        var jwtTokenGenerator = new FakeJwtTokenGenerator();

        var service = new AuthService(
            userRepository,
            passwordHasher,
            jwtTokenGenerator);

        var request = new LoginRequest
        {
            Username = "inactive.user",
            Password = "TestPassword123!"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(request));
    }
}