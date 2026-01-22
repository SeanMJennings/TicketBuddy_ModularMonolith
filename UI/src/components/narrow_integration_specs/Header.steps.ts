import { vi, beforeEach, afterEach, expect } from "vitest";
import { AnOidcCustomerUser } from "../../testing/data";
import {
    renderHeader,
    unmountHeader,
    notificationBellIsRendered,
    notificationBadgeIsRendered,
    getBadgeText,
    clickNotificationBell,
    notificationDropdownIsRendered
} from "./Header.page.tsx";

let mockIsAuthenticated = true;
let mockUser: unknown = AnOidcCustomerUser;
let mockNotificationCount: number | null = 0;

vi.mock("react-oidc-context", () => ({
    useAuth: () => ({
        isAuthenticated: mockIsAuthenticated,
        user: mockUser,
        signinRedirect: async () => {},
        signoutRedirect: async () => {}
    })
}));

vi.mock("../../hooks/useNotificationCount", () => ({
    useNotificationCount: () => ({
        count: mockNotificationCount,
        isLoading: false,
        error: null,
        refetch: () => {}
    })
}));

vi.mock("../../hooks/useNotifications", () => ({
    useNotifications: () => ({
        notifications: [],
        isLoading: false,
        error: null,
        refetch: () => {}
    })
}));

vi.mock("../../api/notifications.api", () => ({
    markNotificationAsRead: vi.fn()
}));

beforeEach(() => {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    mockNotificationCount = 0;
});

afterEach(() => {
    unmountHeader();
});

export function should_show_notification_bell_when_authenticated() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;

    renderHeader();

    expect(notificationBellIsRendered()).toBe(true);
}

export function should_not_show_notification_bell_when_not_authenticated() {
    mockIsAuthenticated = false;
    mockUser = null;

    renderHeader();

    expect(notificationBellIsRendered()).toBe(false);
}

export function should_show_badge_with_unread_count_when_count_greater_than_zero() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    mockNotificationCount = 5;

    renderHeader();

    expect(notificationBadgeIsRendered()).toBe(true);
    expect(getBadgeText()).toBe("5");
}

export function should_not_show_badge_when_count_is_zero() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    mockNotificationCount = 0;

    renderHeader();

    expect(notificationBadgeIsRendered()).toBe(false);
}

export function should_not_show_badge_when_count_is_null() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;
    mockNotificationCount = null;

    renderHeader();

    expect(notificationBadgeIsRendered()).toBe(false);
}

export function should_not_show_dropdown_initially() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;

    renderHeader();

    expect(notificationDropdownIsRendered()).toBe(false);
}

export async function should_show_dropdown_when_bell_clicked() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;

    renderHeader();
    await clickNotificationBell();

    expect(notificationDropdownIsRendered()).toBe(true);
}

export async function should_hide_dropdown_when_bell_clicked_again() {
    mockIsAuthenticated = true;
    mockUser = AnOidcCustomerUser;

    renderHeader();
    await clickNotificationBell();
    await clickNotificationBell();

    expect(notificationDropdownIsRendered()).toBe(false);
}
