namespace DeployNexus.Application.Authentication.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}