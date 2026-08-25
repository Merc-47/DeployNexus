using DeployNexus.Application.Dashboard.DTOs;
using DeployNexus.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DeployNexusDbContext _context;

    public DashboardController(
        DeployNexusDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        var result = new DashboardSummaryDto
        {
            Users = await _context.Users.CountAsync(),

            Organizations =
                await _context.Organizations.CountAsync(),

            Roles =
                await _context.Roles.CountAsync(),

            Permissions =
                await _context.Permissions.CountAsync(),

            Modules =
                await _context.Modules.CountAsync()
        };

        return Ok(result);
    }
}