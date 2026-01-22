import { vi, beforeEach, afterEach, expect } from "vitest";
import { renderHook, waitFor, cleanup } from "@testing-library/react";
import { MockServer } from "../../testing/mock-server";
import { useNotifications } from "../useNotifications";
import { NotificationsForFirstUser } from "../../testing/data";

const mockServer = MockServer.New();

let mockIsAuthenticated = true;
let mockAccessToken: string | undefined = "test-jwt-token";

vi.mock("react-oidc-context", () => ({
    useAuth: () => ({
        isAuthenticated: mockIsAuthenticated,
        user: mockAccessToken ? { access_token: mockAccessToken } : null
    })
}));

beforeEach(() => {
    mockIsAuthenticated = true;
    mockAccessToken = "test-jwt-token";
    mockServer.reset();
    mockServer.start();
});

afterEach(() => {
    cleanup();
});

export async function should_fetch_notifications_on_mount() {
    mockServer.get("/notifications", NotificationsForFirstUser);

    const { result } = renderHook(() => useNotifications());

    await waitFor(() => {
        expect(result.current.notifications).toHaveLength(2);
    });

    expect(result.current.notifications[0].Id).toBe(NotificationsForFirstUser[0].Id);
    expect(result.current.notifications[1].Id).toBe(NotificationsForFirstUser[1].Id);
}

export async function should_return_empty_array_when_no_notifications() {
    mockServer.get("/notifications", []);

    const { result } = renderHook(() => useNotifications());

    await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.notifications).toHaveLength(0);
}

export async function should_not_fetch_when_not_authenticated() {
    mockIsAuthenticated = false;
    mockAccessToken = undefined;

    const wasCalled = mockServer.get("/notifications", NotificationsForFirstUser);

    const { result } = renderHook(() => useNotifications());

    await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.notifications).toHaveLength(0);
    expect(wasCalled()).toBe(false);
}

export async function should_return_loading_state_initially() {
    mockServer.get("/notifications", NotificationsForFirstUser, 100);

    const { result } = renderHook(() => useNotifications());

    expect(result.current.isLoading).toBe(true);

    await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
    });
}

export async function should_pass_authorization_header() {
    const wasCalled = mockServer.get("/notifications", NotificationsForFirstUser);

    const { result } = renderHook(() => useNotifications());

    await waitFor(() => {
        expect(result.current.notifications).toHaveLength(2);
    });

    await waitFor(() => {
        expect(wasCalled()).toBe(true);
    });

    expect(mockServer.headers.get("Authorization")).toBe("Bearer test-jwt-token");
}

export async function should_provide_refetch_function() {
    mockServer.get("/notifications", NotificationsForFirstUser);

    const { result } = renderHook(() => useNotifications());

    await waitFor(() => {
        expect(result.current.notifications).toHaveLength(2);
    });

    expect(typeof result.current.refetch).toBe("function");
}

export async function should_identify_unread_notifications() {
    mockServer.get("/notifications", NotificationsForFirstUser);

    const { result } = renderHook(() => useNotifications());

    await waitFor(() => {
        expect(result.current.notifications).toHaveLength(2);
    });

    const unread = result.current.notifications.filter(n => !n.IsRead);
    const read = result.current.notifications.filter(n => n.IsRead);

    expect(unread).toHaveLength(1);
    expect(read).toHaveLength(1);
}
