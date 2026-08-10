using DeployNexus.Application.Authentication.DTOs;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Authentication.Interfaces;

public interface IJwtTokenGenerator
{
    LoginResponse GenerateToken(User user);
}