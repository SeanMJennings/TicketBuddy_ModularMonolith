import {afterEach, beforeEach, expect, vi} from "vitest";
import {
    protectedContentIsRendered,
    redirectedToHomePage,
    renderProtectedRoute, showsLoadingPage, unrenderProtectedRoute
} from "./ProtectedRoute.page";
import {AnOidcCustomerUser} from "../../testing/data.ts";
import {type OidcUser, UserType} from "../../domain/user.ts";

vi.resetModules();

let authenticated = false;
let user: OidcUser | null = null;
let isLoading = false;

vi.mock('react-oidc-context', () => {
    return {
        useAuth: () => ({
            isAuthenticated: authenticated,
            user: user,
            isLoading: isLoading,
        }),
    };
});

beforeEach(() => {
    authenticated = true;
    user = AnOidcCustomerUser;
    isLoading = false;
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
    expect(redirectedToHomePage()).toBe(true);
}

export async function should_not_render_protected_content_for_wrong_user_type() {
    renderProtectedRoute({requiredType: UserType.Administrator});
    expect(protectedContentIsRendered()).toBe(false);
    expect(redirectedToHomePage()).toBe(true);
}

export async function should_render_protected_content_for_authorized_user_of_any_type() {
    renderProtectedRoute({});
    expect(protectedContentIsRendered()).toBe(true);
}

export async function renders_loading_when_loading() {
    isLoading = true;
    renderProtectedRoute({});
    expect(showsLoadingPage()).toBe(true);
}

