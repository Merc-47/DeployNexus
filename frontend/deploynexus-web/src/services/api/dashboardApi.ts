import { apiClient } from "./apiClient";

export interface DashboardSummary {
    users: number;
    organizations: number;
    roles: number;
    permissions: number;
    modules: number;
}

export async function getDashboardSummary(): Promise<DashboardSummary> {
    return apiClient<DashboardSummary>(
        "/api/Dashboard/summary"
    );
}