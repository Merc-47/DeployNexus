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


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrganizationRequest request)
    {
        var result = await _organizationService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizations = await _organizationService.GetAllAsync();

        return Ok(organizations);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var organization =
            await _organizationService.GetByIdAsync(id);

        if (organization == null)
            return NotFound();

        return Ok(organization);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
       Guid id,
       UpdateOrganizationRequest request)
    {
        var organization =
            await _organizationService.UpdateAsync(id, request);

        if (organization == null)
            return NotFound();

        return Ok(organization);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result =
            await _organizationService.DeactivateAsync(id);

        if (!result.Success)
            return NotFound(result.Message);

        return NoContent();
    }
}