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

    public UsersController(
        IUserService userService)
    {
        _userService = userService;
    }


    // ============================================================
    // CREATE USER
    // ============================================================

    [RequirePermission("USER_CREATE")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateUserRequest request)
    {
        var result =
            await _userService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }


    // ============================================================
    // GET ALL USERS
    // ============================================================

    [RequirePermission("USER_VIEW")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users =
            await _userService.GetAllAsync();

        return Ok(users);
    }


    // ============================================================
    // GET USER BY ID
    // ============================================================

    [RequirePermission("USER_VIEW")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var user =
            await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }


    // ============================================================
    // GET INACTIVE USERS
    // ============================================================

    [RequirePermission("USER_VIEW")]
    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveUsers()
    {
        var users =
            await _userService.GetInactiveUsersAsync();

        return Ok(users);
    }


    // ============================================================
    // UPDATE USER
    // ============================================================

    [RequirePermission("USER_UPDATE")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateUserRequest request)
    {
        var result =
            await _userService.UpdateAsync(
                id,
                request);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }


    // ============================================================
    // ASSIGN / REMOVE USER ROLE
    // ============================================================

    [RequirePermission("USER_ROLE_ASSIGN")]
    [HttpPut("{id}/role")]
    public async Task<IActionResult> AssignRole(
        Guid id,
        AssignUserRoleRequest request)
    {
        var result =
            await _userService.AssignRoleAsync(
                id,
                request);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }


    // ============================================================
    // DEACTIVATE USER
    // ============================================================

    [RequirePermission("USER_DELETE")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(
        Guid id)
    {
        var result =
            await _userService.DeactivateAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return NoContent();
    }
}