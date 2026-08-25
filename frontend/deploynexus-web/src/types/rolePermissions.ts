export interface RolePermission {
    id: string;
    name: string;
    code: string;
    moduleId: string;
    isActive: boolean;
    isAssigned: boolean;
}

export interface AssignedRolePermission {
    roleId: string;
    permissionId: string;
    permissionName: string;
    permissionCode: string;
}

export interface UpdateRolePermissionsRequest {
    permissionIds: string[];
}