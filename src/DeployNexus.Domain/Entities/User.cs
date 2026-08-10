using DeployNexus.Domain.Common;

namespace DeployNexus.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;


    public Guid OrganizationId { get; set; }

    public Organization Organization { get; set; } = null!;


    public Guid? RoleId { get; set; }

    public Role? Role { get; set; }
}