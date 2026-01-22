import { vi, beforeEach, afterEach, expect } from "vitest";
import React from "react";
import { MockServer } from "../../testing/mock-server";
import { waitUntil } from "../../testing/utilities";
import { OidcUsers, NotificationsForFirstUser, createNotification } from "../../testing/data";
import {
    renderNotificationDropdown,
    unmountNotificationDropdown,
    getNotificationItemCount,
    getNotificationText,
    emptyStateIsRendered,
    loadingStateIsRendered,
    isNotificationUnread,
    clickNotification
} from "./NotificationDropdown.page.tsx";

const mockServer = MockServer.New();
let wait_for_get_notifications: () => boolean;
let wait_for_mark_as_read: () => boolean;

vi.mock("react-oidc-context", () => ({
    AuthProvider: ({ children }: { children?: React.ReactNode }) => {
        return React.createElement(React.Fragment, null, children);
    },
    useAuth: () => ({
        isAuthenticated: true,
        user: OidcUsers[0],
        signinRedirect: async () => {},
        signoutRedirect: async () => {}
    })
}));

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual("react-router-dom");
    return {
        ...actual,
        useNavigate: () => mockNavigate
    };
});

beforeEach(() => {
    mockServer.reset();
    mockNavigate.mockReset();
});

afterEach(() => {
    unmountNotificationDropdown();
});

export async function should_render_list_of_notifications() {
    wait_for_get_notifications = mockServer.get("/notifications", NotificationsForFirstUser);
    mockServer.start();

    renderNotificationDropdown();
    await waitUntil(wait_for_get_notifications);

    expect(getNotificationItemCount()).toBe(2);
}

export async function should_show_event_name_for_ticket_purchased_notification() {
    const notification = createNotification({
        Id: "n-test-1",
        Payload: JSON.stringify({ eventId: "e1", ticketId: "t1", eventName: "Rock Festival 2026" })
    });
    wait_for_get_notifications = mockServer.get("/notifications", [notification]);
    mockServer.start();

    renderNotificationDropdown();
    await waitUntil(wait_for_get_notifications);

    expect(getNotificationText(0)).toContain("Rock Festival 2026");
}

export async function should_show_empty_state_when_no_notifications() {
    wait_for_get_notifications = mockServer.get("/notifications", []);
    mockServer.start();

    renderNotificationDropdown();
    await waitUntil(wait_for_get_notifications);

    expect(emptyStateIsRendered()).toBe(true);
}

export function should_show_loading_state_while_fetching() {
    wait_for_get_notifications = mockServer.get("/notifications", [], 5000);
    mockServer.start();

    renderNotificationDropdown();

    expect(loadingStateIsRendered()).toBe(true);
}

export async function should_highlight_unread_notifications() {
    const notifications = [
        createNotification({ Id: "notif-1", IsRead: false }),
        createNotification({ Id: "notif-2", IsRead: true })
    ];
    wait_for_get_notifications = mockServer.get("/notifications", notifications);
    mockServer.start();

    renderNotificationDropdown();
    await waitUntil(wait_for_get_notifications);

    expect(isNotificationUnread(0)).toBe(true);
    expect(isNotificationUnread(1)).toBe(false);
}

export async function should_call_mark_as_read_when_notification_clicked() {
    const notification = createNotification({ Id: "notif-123", IsRead: false });
    wait_for_get_notifications = mockServer.get("/notifications", [notification]);
    wait_for_mark_as_read = mockServer.post("/notifications/notif-123/read", undefined, 204);
    mockServer.start();

    renderNotificationDropdown();
    await waitUntil(wait_for_get_notifications);
    await clickNotification(0);
    await waitUntil(wait_for_mark_as_read);

    expect(mockServer.headers.get("Authorization")).toBe(`Bearer ${OidcUsers[0].access_token}`);
}

export async function should_refetch_notifications_after_marking_as_read() {
    const notification = createNotification({ Id: "notif-123", IsRead: false });
    wait_for_get_notifications = mockServer.get("/notifications", [notification]);
    wait_for_mark_as_read = mockServer.post("/notifications/notif-123/read", undefined, 204);
    mockServer.start();

    renderNotificationDropdown();
    await waitUntil(wait_for_get_notifications);

    mockServer.reset();
    const updatedNotification = createNotification({ Id: "notif-123", IsRead: true });
    const wait_for_refetch = mockServer.get("/notifications", [updatedNotification]);
    wait_for_mark_as_read = mockServer.post("/notifications/notif-123/read", undefined, 204);
    mockServer.start();

    await clickNotification(0);
    await waitUntil(wait_for_mark_as_read);
    await waitUntil(wait_for_refetch);

    expect(isNotificationUnread(0)).toBe(false);
}

export async function should_navigate_to_tickets_page_when_ticket_purchased_notification_clicked() {
    const notification = createNotification({
        Id: "notif-123",
        Type: "TicketPurchased",
        Payload: JSON.stringify({ eventId: "event-456", ticketId: "ticket-789", eventName: "Rock Concert" })
    });
    wait_for_get_notifications = mockServer.get("/notifications", [notification]);
    wait_for_mark_as_read = mockServer.post("/notifications/notif-123/read", undefined, 204);
    mockServer.start();

    renderNotificationDropdown();
    await waitUntil(wait_for_get_notifications);
    await clickNotification(0);
    await waitUntil(wait_for_mark_as_read);

    expect(mockNavigate).toHaveBeenCalledWith("/tickets/event-456");
}
