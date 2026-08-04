using DeployNexus.Application.Roles.DTOs;
using DeployNexus.Application.Roles.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeployNexus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;


    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleRequest request)
    {
        var result = await _roleService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleService.GetAllAsync();

        return Ok(roles);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);

        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateRoleRequest request)
    {
        var result = await _roleService.UpdateAsync(id, request);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _roleService.DeactivateAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return NoContent();
    }
}