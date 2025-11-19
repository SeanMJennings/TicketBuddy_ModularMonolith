export interface User {
    Id: string;
    FullName: string;
    Email: string;
    UserType: UserType;
}

export interface OidcUser {
    profile: { sub: string; name: string; email: string; email_verified: boolean };
    id_token: string;
    access_token: string;
    token_type: string;
    scope: string;
    expires_at: number;
    session_state: null
}

export enum UserType{
    Customer= "Customer",
    Administrator = "Administrator",
}