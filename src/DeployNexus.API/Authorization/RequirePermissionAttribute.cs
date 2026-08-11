using Microsoft.AspNetCore.Authorization;

namespace DeployNexus.API.Authorization;

public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permissionCode)
    {
        Policy = $"Permission:{permissionCode}";
    }
}