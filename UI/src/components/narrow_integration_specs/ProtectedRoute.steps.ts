import {afterEach, beforeEach, expect, vi} from "vitest";
import {
    protectedContentIsRendered,
    redirectedToLogin,
    redirectedToUnauthorized,
    renderProtectedRoute, unrenderProtectedRoute
} from "./ProtectedRoute.page";
import React from "react";
import {AnOidcCustomerUser} from "../../testing/data.ts";
import {type OidcUser, UserType} from "../../domain/user.ts";

vi.resetModules();

let authenticated = false;
let user: OidcUser | null = null;

vi.mock('react-oidc-context', () => {
    return {
        AuthProvider: ({ children }: { children?: React.ReactNode }) => {
            return React.createElement(React.Fragment, null, children);
        },
        useAuth: () => ({
            isAuthenticated: authenticated,
            user: user,
            signinRedirect: async () => {},
            signoutRedirect: async () => {},
        }),
    };
});

beforeEach(() => {
    authenticated = true;
    user = AnOidcCustomerUser;
});

afterEach(() => unrenderProtectedRoute());

export async function should_render_protected_content_for_authorized_user_of_correct_type() {
    renderProtectedRoute({requiredType: UserType.Customer});
    expect(protectedContentIsRendered()).toBe(true);
}

export async function should_not_render_protected_content_for_unauthenticated_user() {
    authenticated = false;
    user = null;
    renderProtectedRoute({});
    expect(protectedContentIsRendered()).toBe(false);
    expect(redirectedToLogin()).toBe(true);
}

export async function should_not_render_protected_content_for_wrong_user_type() {
    renderProtectedRoute({requiredType: UserType.Administrator});
    expect(protectedContentIsRendered()).toBe(false);
    expect(redirectedToUnauthorized()).toBe(true);
}

export async function should_render_protected_content_for_authorized_user_of_any_type() {
    renderProtectedRoute({});
    expect(protectedContentIsRendered()).toBe(true);
}

