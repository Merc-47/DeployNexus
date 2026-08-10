using DeployNexus.API.Authorization;
using DeployNexus.Application.Users.DTOs;
using DeployNexus.Application.Users.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeployNexus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [RequirePermission("USER_VIEW")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    [RequirePermission("USER_VIEW")]
    [HttpGet("inactive")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetInactiveUsers()
    {
        var users = await _userService.GetInactiveUsersAsync();

        return Ok(users);
    }

    [RequirePermission("USER_VIEW")]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [RequirePermission("USER_CREATE")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            user
        );
    }

    [RequirePermission("USER_UPDATE")]
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(
        Guid id,
        UpdateUserRequest request)
    {
        var user = await _userService.UpdateAsync(id, request);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [RequirePermission("USER_DELETE")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _userService.DeactivateAsync(id);

        if (!result.Success)
        {
            return NotFound(result.Message);
        }

        return NoContent();
    }
}