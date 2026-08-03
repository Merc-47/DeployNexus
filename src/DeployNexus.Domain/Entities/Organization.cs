using DeployNexus.Domain.Common;

namespace DeployNexus.Domain.Entities;

public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;


    public ICollection<User> Users { get; set; } = new List<User>();
}