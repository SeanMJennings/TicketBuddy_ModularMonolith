import {User as OidcUser} from "oidc-client-ts"
import {type User, UserType} from "../domain/user.ts";
import {decodeJwtClaims, extractScopes} from "./jwt.ts";

export function convertToTicketBuddyUser(user: OidcUser | null | undefined) {
    if (!user) {
        return null;
    }
    return {
        Id: user?.profile?.sub,
        FullName: user?.profile?.name ?? "",
        Email: user?.profile?.email ?? "",
        UserType: mapUserType(user?.access_token)
    }
}

export function isALoggedInCustomer(user: User | null): boolean {
    return user?.UserType === UserType.Customer;
}

const mapUserType = (accessToken: string | undefined): UserType => {
    const claims = decodeJwtClaims(accessToken);
    const scopes = extractScopes(claims);

    switch (scopes?.find(scope => scope?.startsWith("ticketbuddy-"))) {
        case "ticketbuddy-admin":
            return UserType.Administrator;
        case "ticketbuddy-customer":
            return UserType.Customer;
        default:
            throw new Error(`Unknown user type`);
    }
}