using DeployNexus.Domain.Common;

namespace DeployNexus.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Guid OrganizationId { get; set; }

    public Organization Organization { get; set; } = null!;

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}