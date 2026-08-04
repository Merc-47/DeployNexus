namespace DeployNexus.Application.Roles.DTOs;

public class RoleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public Guid OrganizationId { get; set; }
}