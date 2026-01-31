import { vi, beforeEach, afterEach, expect } from "vitest";
import React from "react";
import { MockServer } from "../../testing/mock-server";
import { waitUntil } from "../../testing/utilities";
import {AnOidcAdminUser, AnOidcCustomerUser} from "../../testing/data";
import {
    renderHeader,
    unmountHeader,
    notificationBellIsRendered,
    notificationBadgeIsRendered,
    getBadgeText,
    clickNotificationBell,
    notificationDropdownIsRendered
} from "./Header.page.tsx";

const mockServer = MockServer.New();
let wait_for_get_unread_count: () => boolean;
let wait_for_get_notifications: () => boolean;

let mockIsAuthenticated = true;
let mockUser: unknown = AnOidcCustomerUser;

vi.mock("react-oidc-context", () => ({
    AuthProvider: ({ children }: { children?: React.ReactNode }) => {
        return React.createElement(React.Fragment, null, children);
    },
    useAuth: () => ({
        isAuthenticated: mockIsAuthenticated,
        user: mockUser,
        signinRedirect: async () => {},
        signoutRedirect: async () => {}
    })
}));

beforeEach(() => {
    mockServer.reset();
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
});

afterEach(() => {
    unmountHeader();
});

export async function should_show_notification_bell_when_authenticated() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    wait_for_get_unread_count = mockServer.get("/notifications/unread-count", { Count: 0 });
    mockServer.start();

    renderHeader();
    await waitUntil(wait_for_get_unread_count);

    expect(notificationBellIsRendered()).toBe(true);
}

export async function should_not_show_notification_bell_when_not_authenticated() {
    mockIsAuthenticated = false;
    mockUser = null;
    mockServer.start();

    renderHeader();

    expect(notificationBellIsRendered()).toBe(false);
}

export async function should_not_show_notification_bell_when_user_is_an_admin() {
    mockIsAuthenticated = true;
    mockUser = AnOidcAdminUser;
    mockServer.start();

    renderHeader();

    expect(notificationBellIsRendered()).toBe(false);
}

export async function should_show_badge_with_unread_count_when_count_greater_than_zero() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    wait_for_get_unread_count = mockServer.get("/notifications/unread-count", { Count: 5 });
    mockServer.start();

    renderHeader();
    await waitUntil(wait_for_get_unread_count);

    expect(notificationBadgeIsRendered()).toBe(true);
    expect(getBadgeText()).toBe("5");
}

export async function should_not_show_badge_when_count_is_zero() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    wait_for_get_unread_count = mockServer.get("/notifications/unread-count", { Count: 0 });
    mockServer.start();

    renderHeader();
    await waitUntil(wait_for_get_unread_count);

    expect(notificationBadgeIsRendered()).toBe(false);
}

export async function should_not_show_badge_when_api_returns_error() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    wait_for_get_unread_count = mockServer.get("/notifications/unread-count", { error: "Not found" }, undefined, 404);
    mockServer.start();

    renderHeader();
    await waitUntil(wait_for_get_unread_count);

    expect(notificationBadgeIsRendered()).toBe(false);
}

export async function should_not_show_dropdown_initially() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    wait_for_get_unread_count = mockServer.get("/notifications/unread-count", { Count: 0 });
    mockServer.start();

    renderHeader();
    await waitUntil(wait_for_get_unread_count);

    expect(notificationDropdownIsRendered()).toBe(false);
}

export async function should_show_dropdown_when_bell_clicked() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    wait_for_get_unread_count = mockServer.get("/notifications/unread-count", { Count: 0 });
    wait_for_get_notifications = mockServer.get("/notifications", []);
    mockServer.start();

    renderHeader();
    await waitUntil(wait_for_get_unread_count);
    await clickNotificationBell();
    await waitUntil(wait_for_get_notifications);

    expect(notificationDropdownIsRendered()).toBe(true);
}

export async function should_hide_dropdown_when_bell_clicked_again() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    wait_for_get_unread_count = mockServer.get("/notifications/unread-count", { Count: 0 });
    wait_for_get_notifications = mockServer.get("/notifications", []);
    mockServer.start();

    renderHeader();
    await waitUntil(wait_for_get_unread_count);
    await clickNotificationBell();
    await waitUntil(wait_for_get_notifications);
    await clickNotificationBell();

    expect(notificationDropdownIsRendered()).toBe(false);
}
