import {type ReactNode} from "react";
import {Navigate} from "react-router-dom";
import {useAuth} from "react-oidc-context";
import {convertToTicketBuddyUser} from "../oidc/key-cloak-user.extensions";
import type {UserType} from "../domain/user.ts";

type ProtectedRouteProps = {
    requiredUserType: UserType;
    children: ReactNode;
};

export function ProtectedRoute({requiredUserType, children}: ProtectedRouteProps) {
    const auth = useAuth();
    const user = convertToTicketBuddyUser(auth.user);

    if (!user) return <Navigate to="/" replace />;
    if (user.UserType !== requiredUserType) return <Navigate to="/unauthorized" replace />;
    return <>{children}</>;
}

