namespace DeployNexus.Application.RolePermissions.DTOs;

public class RolePermissionDto
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public string PermissionName { get; set; } = string.Empty;

    public string PermissionCode { get; set; } = string.Empty;
}