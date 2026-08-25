import { apiClient } from "./apiClient";

import type {
    Role,
    CreateRoleRequest,
    UpdateRoleRequest,
    Organization,
} from "../../types/roles";


// ============================================================
// Roles
// ============================================================

export async function getRoles(): Promise<Role[]> {
    return apiClient<Role[]>("/api/Roles");
}


export async function getRole(
    id: string
): Promise<Role> {
    return apiClient<Role>(
        `/api/Roles/${id}`
    );
}


export async function createRole(
    request: CreateRoleRequest
): Promise<Role> {
    return apiClient<Role>(
        "/api/Roles",
        {
            method: "POST",
            body: JSON.stringify(request),
        }
    );
}


export async function updateRole(
    id: string,
    request: UpdateRoleRequest
): Promise<Role> {
    return apiClient<Role>(
        `/api/Roles/${id}`,
        {
            method: "PUT",
            body: JSON.stringify(request),
        }
    );
}


export async function deactivateRole(
    id: string
): Promise<void> {
    return apiClient<void>(
        `/api/Roles/${id}`,
        {
            method: "DELETE",
        }
    );
}


// ============================================================
// Organizations
// ============================================================

export async function getOrganizations(): Promise<
    Organization[]
> {
    return apiClient<Organization[]>(
        "/api/Organizations"
    );
}