using DeployNexus.Domain.Enums;

namespace DeployNexus.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    Guid? OrganizationId { get; }

    Guid? RoleId { get; }

    RoleType? RoleType { get; }

    bool IsAuthenticated { get; }

    bool IsSystemUser { get; }
}