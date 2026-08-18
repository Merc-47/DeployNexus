using DeployNexus.Domain.Entities;
using DeployNexus.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        DeployNexusDbContext context)
    {
        await SeedModulesAsync(context);

        await SeedPermissionsAsync(context);
    }


    // ============================================================
    // Modules
    // ============================================================

    private static async Task SeedModulesAsync(
        DeployNexusDbContext context)
    {
        var modules = new[]
        {
            new Module
            {
                Id = Guid.NewGuid(),
                Name = "Users",
                Code = "USERS",
                Description = "User management",
                Status = ModuleStatus.Available
            },

            new Module
            {
                Id = Guid.NewGuid(),
                Name = "Organizations",
                Code = "ORGANIZATIONS",
                Description = "Organization management",
                Status = ModuleStatus.Available
            },

            new Module
            {
                Id = Guid.NewGuid(),
                Name = "Roles",
                Code = "ROLES",
                Description = "Role management",
                Status = ModuleStatus.Available
            },

            new Module
            {
                Id = Guid.NewGuid(),
                Name = "Permissions",
                Code = "PERMISSIONS",
                Description = "Permission management",
                Status = ModuleStatus.Available
            },

            new Module
            {
                Id = Guid.NewGuid(),
                Name = "Modules",
                Code = "MODULES",
                Description = "Module management",
                Status = ModuleStatus.Available
            }
        };


        foreach (var module in modules)
        {
            var exists =
                await context.Modules
                    .AnyAsync(x => x.Code == module.Code);

            if (!exists)
            {
                await context.Modules.AddAsync(module);
            }
        }


        await context.SaveChangesAsync();
    }


    // ============================================================
    // Permissions
    // ============================================================

    private static async Task SeedPermissionsAsync(
        DeployNexusDbContext context)
    {
        var modules =
            await context.Modules
                .ToDictionaryAsync(x => x.Code);


        var permissions = new[]
        {
            new
            {
                Name = "View Users",
                Code = "USER_VIEW",
                ModuleCode = "USERS"
            },

            new
            {
                Name = "Create Users",
                Code = "USER_CREATE",
                ModuleCode = "USERS"
            },

            new
            {
                Name = "Update Users",
                Code = "USER_UPDATE",
                ModuleCode = "USERS"
            },

            new
            {
                Name = "Delete Users",
                Code = "USER_DELETE",
                ModuleCode = "USERS"
            },


            new
            {
                Name = "View Organizations",
                Code = "ORGANIZATION_VIEW",
                ModuleCode = "ORGANIZATIONS"
            },

            new
            {
                Name = "Create Organizations",
                Code = "ORGANIZATION_CREATE",
                ModuleCode = "ORGANIZATIONS"
            },

            new
            {
                Name = "Update Organizations",
                Code = "ORGANIZATION_UPDATE",
                ModuleCode = "ORGANIZATIONS"
            },

            new
            {
                Name = "Delete Organizations",
                Code = "ORGANIZATION_DELETE",
                ModuleCode = "ORGANIZATIONS"
            },


            new
            {
                Name = "View Roles",
                Code = "ROLE_VIEW",
                ModuleCode = "ROLES"
            },

            new
            {
                Name = "Create Roles",
                Code = "ROLE_CREATE",
                ModuleCode = "ROLES"
            },

            new
            {
                Name = "Update Roles",
                Code = "ROLE_UPDATE",
                ModuleCode = "ROLES"
            },

            new
            {
                Name = "Delete Roles",
                Code = "ROLE_DELETE",
                ModuleCode = "ROLES"
            },


            new
            {
                Name = "View Permissions",
                Code = "PERMISSION_VIEW",
                ModuleCode = "PERMISSIONS"
            },

            new
            {
                Name = "Create Permissions",
                Code = "PERMISSION_CREATE",
                ModuleCode = "PERMISSIONS"
            },

            new
            {
                Name = "Update Permissions",
                Code = "PERMISSION_UPDATE",
                ModuleCode = "PERMISSIONS"
            },

            new
            {
                Name = "Delete Permissions",
                Code = "PERMISSION_DELETE",
                ModuleCode = "PERMISSIONS"
            },


            new
            {
                Name = "View Modules",
                Code = "MODULE_VIEW",
                ModuleCode = "MODULES"
            },

            new
            {
                Name = "Create Modules",
                Code = "MODULE_CREATE",
                ModuleCode = "MODULES"
            },

            new
            {
                Name = "Update Modules",
                Code = "MODULE_UPDATE",
                ModuleCode = "MODULES"
            },

            new
            {
                Name = "Disable Modules",
                Code = "MODULE_DISABLE",
                ModuleCode = "MODULES"
            }
        };


        foreach (var permission in permissions)
        {
            var exists =
                await context.Permissions
                    .AnyAsync(x =>
                        x.Code == permission.Code);

            if (exists)
            {
                continue;
            }


            if (!modules.TryGetValue(
                    permission.ModuleCode,
                    out var module))
            {
                throw new InvalidOperationException(
                    $"Module '{permission.ModuleCode}' was not found.");
            }


            var entity = new Permission
            {
                Id = Guid.NewGuid(),
                Name = permission.Name,
                Code = permission.Code,
                IsActive = true,
                ModuleId = module.Id
            };


            await context.Permissions.AddAsync(entity);
        }


        await context.SaveChangesAsync();
    }
}