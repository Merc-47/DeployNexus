export interface Role {
    id: string;
    name: string;
    description: string;
    isActive: boolean;
    organizationId: string;
}

export interface CreateRoleRequest {
    name: string;
    description: string;
    organizationId: string;
}

export interface UpdateRoleRequest {
    name: string;
    description: string;
}

export interface Organization {
    id: string;
    name: string;
    code: string;
    isActive: boolean;
}