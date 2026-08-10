using DeployNexus.Application.Authentication.DTOs;

namespace DeployNexus.Application.Authentication.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}