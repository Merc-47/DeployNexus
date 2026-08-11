using DeployNexus.Application.Authentication.DTOs;
using DeployNexus.Application.Authentication.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Tests.Authentication;

public class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public LoginResponse GenerateToken(User user)
    {
        return new LoginResponse
        {
            Token = $"fake-token-{user.Id}",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}