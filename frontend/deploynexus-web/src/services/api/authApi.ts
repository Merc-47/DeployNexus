import type { LoginRequest, LoginResponse } from "../../types/auth";
import { apiClient } from "./apiClient";

export async function login(
    request: LoginRequest
): Promise<LoginResponse> {

    return apiClient<LoginResponse>(
        "/api/Auth/login",
        {
            method: "POST",
            body: JSON.stringify(request),
        }
    );
}


export async function getCurrentUser(): Promise<CurrentUser> {

    return apiClient<CurrentUser>(
        "/api/Auth/me",
        {
            method: "GET",
        }
    );
}


export interface CurrentUser {
    id: string;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
}