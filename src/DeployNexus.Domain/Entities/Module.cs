using DeployNexus.Domain.Common;
using DeployNexus.Domain.Enums;

namespace DeployNexus.Domain.Entities;

public class Module : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ModuleStatus Status { get; set; }
        = ModuleStatus.Available;

    public ICollection<Permission> Permissions { get; set; }
        = new List<Permission>();
}