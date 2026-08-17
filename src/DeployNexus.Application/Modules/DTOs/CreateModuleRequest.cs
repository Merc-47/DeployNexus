namespace DeployNexus.Application.Modules.DTOs;

public class CreateModuleRequest
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}