using DeployNexus.Application.Modules.DTOs;
using DeployNexus.Application.Modules.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeployNexus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModulesController : ControllerBase
{
    private readonly IModuleService _moduleService;

    public ModulesController(
        IModuleService moduleService)
    {
        _moduleService = moduleService;
    }


    // ============================================================
    // GET: api/Modules
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var modules =
            await _moduleService.GetAllAsync();

        return Ok(modules);
    }


    // ============================================================
    // GET: api/Modules/{id}
    // ============================================================

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var module =
            await _moduleService.GetByIdAsync(id);

        if (module == null)
        {
            return NotFound();
        }

        return Ok(module);
    }


    // ============================================================
    // POST: api/Modules
    // ============================================================

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateModuleRequest request)
    {
        var module =
            await _moduleService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = module.Id },
            module);
    }


    // ============================================================
    // PUT: api/Modules/{id}
    // ============================================================

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateModuleRequest request)
    {
        var module =
            await _moduleService.UpdateAsync(
                id,
                request);

        if (module == null)
        {
            return NotFound();
        }

        return Ok(module);
    }


    // ============================================================
    // DELETE: api/Modules/{id}
    // ============================================================

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Disable(
        Guid id)
    {
        var result =
            await _moduleService.DisableAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return NoContent();
    }
}