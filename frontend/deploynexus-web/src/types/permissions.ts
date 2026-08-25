export interface Permission {

    id: string;

    name: string;

    code: string;

    moduleId: string;

    isActive: boolean;

}


export interface CreatePermissionRequest {

    name: string;

    code: string;

    moduleId: string;

}


export interface UpdatePermissionRequest {

    name: string;

    code: string;

}