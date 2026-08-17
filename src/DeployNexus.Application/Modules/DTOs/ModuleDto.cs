using DeployNexus.Domain.Enums;

namespace DeployNexus.Application.Modules.DTOs;

public class ModuleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ModuleStatus Status { get; set; }
}