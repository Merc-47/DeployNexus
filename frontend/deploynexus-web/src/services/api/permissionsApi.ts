import { apiClient } from "./apiClient";

import type {
    RolePermission,
    AssignedRolePermission,
} from "../../types/rolePermissions";

import type {
    Permission,
    CreatePermissionRequest,
    UpdatePermissionRequest,
} from "../../types/permissions";

// ============================================================
// Get all permissions
// ============================================================

export async function getPermissions(): Promise<RolePermission[]> {

    return apiClient<RolePermission[]>(
        "/api/Permissions"
    );
}


// ============================================================
// Get permissions assigned to a role
// ============================================================

export async function getRolePermissions(
    roleId: string
): Promise<AssignedRolePermission[]> {

    return apiClient<AssignedRolePermission[]>(
        `/api/Roles/${roleId}/permissions`
    );
}


// ============================================================
// Assign permission to role
// ============================================================

export async function assignRolePermission(
    roleId: string,
    permissionId: string
): Promise<void> {

    await apiClient(
        `/api/Roles/${roleId}/permissions`,
        {
            method: "POST",

            body: JSON.stringify({
                permissionId,
            }),
        }
    );
}


// ============================================================
// Remove permission from role
// ============================================================

export async function removeRolePermission(
    roleId: string,
    permissionId: string
): Promise<void> {

    await apiClient(
        `/api/Roles/${roleId}/permissions/${permissionId}`,
        {
            method: "DELETE",
        }
    );
}

// ============================================================
// Create permission
// ============================================================

export async function createPermission(
    request: CreatePermissionRequest
): Promise<Permission> {

    return apiClient<Permission>(
        "/api/Permissions",
        {
            method: "POST",

            body: JSON.stringify(request),
        }
    );
}


// ============================================================
// Get permission by ID
// ============================================================

export async function getPermission(
    id: string
): Promise<Permission> {

    return apiClient<Permission>(
        `/api/Permissions/${id}`
    );
}


// ============================================================
// Update permission
// ============================================================

export async function updatePermission(
    id: string,
    request: UpdatePermissionRequest
): Promise<Permission> {

    return apiClient<Permission>(
        `/api/Permissions/${id}`,
        {
            method: "PUT",

            body: JSON.stringify(request),
        }
    );
}


// ============================================================
// Deactivate permission
// ============================================================

export async function deactivatePermission(
    id: string
): Promise<void> {

    await apiClient(
        `/api/Permissions/${id}`,
        {
            method: "DELETE",
        }
    );
}