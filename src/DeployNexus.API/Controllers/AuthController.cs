using DeployNexus.Application.Authentication.DTOs;
using DeployNexus.Application.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeployNexus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            username = User.FindFirstValue(ClaimTypes.Name),
            email = User.FindFirstValue(ClaimTypes.Email)
        });
    }
}