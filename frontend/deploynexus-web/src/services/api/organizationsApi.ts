import { apiClient } from "./apiClient";

import type {
    Organization,
    CreateOrganizationRequest,
    UpdateOrganizationRequest,
} from "../../types/organization";


// ============================================================
// Get all organizations
// ============================================================

export async function getOrganizations(): Promise<Organization[]> {

    return apiClient<Organization[]>(
        "/api/Organizations"
    );

}


// ============================================================
// Get organization by ID
// ============================================================

export async function getOrganization(
    id: string
): Promise<Organization> {

    return apiClient<Organization>(
        `/api/Organizations/${id}`
    );

}


// ============================================================
// Create organization
// ============================================================

export async function createOrganization(
    request: CreateOrganizationRequest
): Promise<Organization> {

    return apiClient<Organization>(
        "/api/Organizations",
        {
            method: "POST",

            body: JSON.stringify(request),
        }
    );

}


// ============================================================
// Update organization
// ============================================================

export async function updateOrganization(
    id: string,
    request: UpdateOrganizationRequest
): Promise<Organization> {

    return apiClient<Organization>(
        `/api/Organizations/${id}`,
        {
            method: "PUT",

            body: JSON.stringify(request),
        }
    );

}


// ============================================================
// Deactivate organization
// ============================================================

export async function deactivateOrganization(
    id: string
): Promise<void> {

    await apiClient(
        `/api/Organizations/${id}`,
        {
            method: "DELETE",
        }
    );

}