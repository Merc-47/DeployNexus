export interface User {

    id: string;

    username: string;

    email: string;

    firstName: string;

    lastName: string;

    isActive: boolean;

    organizationId: string;

    roleId: string | null;

}


export interface CreateUserRequest {

    username: string;

    email: string;

    password: string;

    firstName: string;

    lastName: string;

    organizationId: string;

}


export interface UpdateUserRequest {

    username: string;

    email: string;

    firstName: string;

    lastName: string;

    isActive: boolean;

    organizationId: string;

}


export interface AssignUserRoleRequest {

    roleId: string | null;

}