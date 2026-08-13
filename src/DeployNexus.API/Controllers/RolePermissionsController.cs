using DeployNexus.API.Authorization;
using DeployNexus.Application.RolePermissions.DTOs;
using DeployNexus.Application.RolePermissions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeployNexus.API.Controllers;

[ApiController]
[Route("api/Roles/{roleId}/permissions")]
public class RolePermissionsController : ControllerBase
{
    private readonly IRolePermissionService _rolePermissionService;

    public RolePermissionsController(
        IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

    [RequirePermission("ROLE_PERMISSION_ASSIGN")]
    [HttpPost]
    public async Task<IActionResult> Assign(
        Guid roleId,
        AssignPermissionRequest request)
    {
        var result = await _rolePermissionService
            .AssignAsync(roleId, request);

        return CreatedAtAction(
            nameof(GetPermissions),
            new { roleId },
            result);
    }

    [RequirePermission("ROLE_PERMISSION_VIEW")]
    [HttpGet]
    public async Task<IActionResult> GetPermissions(
        Guid roleId)
    {
        var result = await _rolePermissionService
            .GetPermissionsAsync(roleId);

        return Ok(result);
    }

    [RequirePermission("ROLE_PERMISSION_REMOVE")]
    [HttpDelete("{permissionId}")]
    public async Task<IActionResult> Remove(
        Guid roleId,
        Guid permissionId)
    {
        var result = await _rolePermissionService
            .RemoveAsync(roleId, permissionId);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}