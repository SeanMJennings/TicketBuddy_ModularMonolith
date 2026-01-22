import { vi, beforeEach, afterEach, expect } from "vitest";
import { renderHook, waitFor, cleanup } from "@testing-library/react";
import { MockServer } from "../../testing/mock-server";
import { useNotificationCount } from "../useNotificationCount";

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

export async function should_fetch_unread_count_on_mount() {
    mockServer.get("/notifications/unread-count", { Count: 5 });

    const { result } = renderHook(() => useNotificationCount());

    await waitFor(() => {
        expect(result.current.count).toBe(5);
    });
}

export async function should_return_zero_when_no_unread_notifications() {
    mockServer.get("/notifications/unread-count", { Count: 0 });

    const { result } = renderHook(() => useNotificationCount());

    await waitFor(() => {
        expect(result.current.count).toBe(0);
    });
}

export async function should_not_fetch_when_not_authenticated() {
    mockIsAuthenticated = false;
    mockAccessToken = undefined;

    const wasCalled = mockServer.get("/notifications/unread-count", { Count: 5 });

    const { result } = renderHook(() => useNotificationCount());

    await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.count).toBe(null);
    expect(wasCalled()).toBe(false);
}

export async function should_return_loading_state_initially() {
    mockServer.get("/notifications/unread-count", { Count: 5 }, 100);

    const { result } = renderHook(() => useNotificationCount());

    expect(result.current.isLoading).toBe(true);

    await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
    });
}

export async function should_pass_authorization_header() {
    const wasCalled = mockServer.get("/notifications/unread-count", { Count: 3 });

    const { result } = renderHook(() => useNotificationCount());

    await waitFor(() => {
        expect(result.current.count).toBe(3);
    });

    await waitFor(() => {
        expect(wasCalled()).toBe(true);
    });

    expect(mockServer.headers.get("Authorization")).toBe("Bearer test-jwt-token");
}

export async function should_provide_refetch_function() {
    mockServer.get("/notifications/unread-count", { Count: 2 });

    const { result } = renderHook(() => useNotificationCount());

    await waitFor(() => {
        expect(result.current.count).toBe(2);
    });

    expect(typeof result.current.refetch).toBe("function");
}
