using DeployNexus.Application.Permissions.DTOs;
using DeployNexus.Application.Permissions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeployNexus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;


    public PermissionsController(
        IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePermissionRequest request)
    {
        var result = await _permissionService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }



    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var permissions = await _permissionService.GetAllAsync();

        return Ok(permissions);
    }



    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var permission =
            await _permissionService.GetByIdAsync(id);


        if (permission == null)
        {
            return NotFound();
        }


        return Ok(permission);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdatePermissionRequest request)
    {
        var permission =
            await _permissionService.UpdateAsync(id, request);


        if (permission == null)
        {
            return NotFound();
        }


        return Ok(permission);
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result =
            await _permissionService.DeactivateAsync(id);


        if (!result.Success)
        {
            return NotFound(result);
        }


        return NoContent();
    }
}