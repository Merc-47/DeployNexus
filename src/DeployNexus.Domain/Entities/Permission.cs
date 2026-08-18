using DeployNexus.Domain.Common;

namespace DeployNexus.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Guid ModuleId { get; set; }

    public Module Module { get; set; } = null!;

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}