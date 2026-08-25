import { apiClient } from "./apiClient";

import type {
    User,
    CreateUserRequest,
    UpdateUserRequest,
    AssignUserRoleRequest,
} from "../../types/user";


// ============================================================
// Get all users
// ============================================================

export async function getUsers(): Promise<User[]> {

    return apiClient<User[]>(
        "/api/Users"
    );

}


// ============================================================
// Get user by ID
// ============================================================

export async function getUser(
    id: string
): Promise<User> {

    return apiClient<User>(
        `/api/Users/${id}`
    );

}


// ============================================================
// Get inactive users
// ============================================================

export async function getInactiveUsers(): Promise<User[]> {

    return apiClient<User[]>(
        "/api/Users/inactive"
    );

}


// ============================================================
// Create user
// ============================================================

export async function createUser(
    request: CreateUserRequest
): Promise<User> {

    return apiClient<User>(
        "/api/Users",
        {
            method: "POST",

            body: JSON.stringify(request),
        }
    );

}


// ============================================================
// Update user
// ============================================================

export async function updateUser(
    id: string,
    request: UpdateUserRequest
): Promise<User> {

    return apiClient<User>(
        `/api/Users/${id}`,
        {
            method: "PUT",

            body: JSON.stringify(request),
        }
    );

}


// ============================================================
// Assign / remove role
// ============================================================

export async function assignUserRole(
    id: string,
    request: AssignUserRoleRequest
): Promise<User> {

    return apiClient<User>(
        `/api/Users/${id}/role`,
        {
            method: "PUT",

            body: JSON.stringify(request),
        }
    );

}


// ============================================================
// Deactivate user
// ============================================================

export async function deactivateUser(
    id: string
): Promise<void> {

    await apiClient(
        `/api/Users/${id}`,
        {
            method: "DELETE",
        }
    );

}