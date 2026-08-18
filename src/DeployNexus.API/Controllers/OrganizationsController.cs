using DeployNexus.API.Authorization;
using DeployNexus.Application.Organizations.DTOs;
using DeployNexus.Application.Organizations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeployNexus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(
        IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }


    // ============================================================
    // CREATE ORGANIZATION
    // ============================================================

    [RequirePermission("ORGANIZATION_CREATE")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrganizationRequest request)
    {
        var result =
            await _organizationService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }


    // ============================================================
    // GET ALL ORGANIZATIONS
    // ============================================================

    [RequirePermission("ORGANIZATION_VIEW")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizations =
            await _organizationService.GetAllAsync();

        return Ok(organizations);
    }


    // ============================================================
    // GET ORGANIZATION BY ID
    // ============================================================

    [RequirePermission("ORGANIZATION_VIEW")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var organization =
            await _organizationService.GetByIdAsync(id);

        if (organization == null)
        {
            return NotFound();
        }

        return Ok(organization);
    }


    // ============================================================
    // UPDATE ORGANIZATION
    // ============================================================

    [RequirePermission("ORGANIZATION_UPDATE")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateOrganizationRequest request)
    {
        var organization =
            await _organizationService.UpdateAsync(
                id,
                request);

        if (organization == null)
        {
            return NotFound();
        }

        return Ok(organization);
    }


    // ============================================================
    // DEACTIVATE ORGANIZATION
    // ============================================================

    [RequirePermission("ORGANIZATION_DELETE")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(
        Guid id)
    {
        var result =
            await _organizationService.DeactivateAsync(id);

        if (!result.Success)
        {
            return NotFound(result.Message);
        }

        return NoContent();
    }
}