export interface Organization {

    id: string;

    name: string;

    code: string;

    isActive: boolean;

}


export interface CreateOrganizationRequest {

    name: string;

    code: string;

}


export interface UpdateOrganizationRequest {

    name: string;

    code: string;

}