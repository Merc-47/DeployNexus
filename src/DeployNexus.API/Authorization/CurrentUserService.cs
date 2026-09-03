using System.Security.Claims;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Enums;

namespace DeployNexus.API.Authorization;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public Guid? UserId
    {
        get
        {
            var value =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirstValue(
                        ClaimTypes.NameIdentifier);

            if (Guid.TryParse(value, out var userId))
            {
                return userId;
            }

            return null;
        }
    }


    public Guid? OrganizationId
    {
        get
        {
            var value =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirstValue("organizationId");

            if (Guid.TryParse(value, out var organizationId))
            {
                return organizationId;
            }

            return null;
        }
    }


    public Guid? RoleId
    {
        get
        {
            var value =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirstValue("roleId");

            if (Guid.TryParse(value, out var roleId))
            {
                return roleId;
            }

            return null;
        }
    }


    public RoleType? RoleType
    {
        get
        {
            var value =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirstValue("roleType");

            if (int.TryParse(value, out var roleType) &&
                Enum.IsDefined(
                    typeof(RoleType),
                    roleType))
            {
                return (RoleType)roleType;
            }

            return null;
        }
    }


    public bool IsAuthenticated
    {
        get
        {
            return _httpContextAccessor
                .HttpContext?
                .User?
                .Identity?
                .IsAuthenticated
                ?? false;
        }
    }


    public bool IsSystemUser
    {
        get
        {
            return RoleType == Domain.Enums.RoleType.System;
        }
    }
}