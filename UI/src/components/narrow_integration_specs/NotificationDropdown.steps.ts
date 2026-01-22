import { vi, afterEach, expect } from "vitest";
import type { Notification } from "../../domain/notification";
import {
    renderNotificationDropdown,
    unmountNotificationDropdown,
    getNotificationItemCount,
    getNotificationText,
    emptyStateIsRendered,
    loadingStateIsRendered,
    isNotificationUnread
} from "./NotificationDropdown.page.tsx";

let mockNotifications: Notification[] = [];
let mockIsLoading = false;

vi.mock("../../hooks/useNotifications", () => ({
    useNotifications: () => ({
        notifications: mockNotifications,
        isLoading: mockIsLoading,
        error: null,
        refetch: () => {}
    })
}));

afterEach(() => {
    mockNotifications = [];
    mockIsLoading = false;
    unmountNotificationDropdown();
});

const createNotification = (overrides: Partial<Notification> = {}): Notification => ({
    Id: "notif-1",
    UserId: "user-1",
    Type: "TicketPurchased",
    Payload: JSON.stringify({ eventId: "event-1", ticketId: "ticket-1", eventName: "Summer Concert" }),
    IsRead: false,
    CreatedAt: "2026-01-22T10:00:00Z",
    ...overrides
});

export function should_render_list_of_notifications() {
    mockNotifications = [
        createNotification({ Id: "notif-1" }),
        createNotification({ Id: "notif-2" }),
        createNotification({ Id: "notif-3" })
    ];

    renderNotificationDropdown();

    expect(getNotificationItemCount()).toBe(3);
}

export function should_show_event_name_for_ticket_purchased_notification() {
    mockNotifications = [
        createNotification({
            Payload: JSON.stringify({ eventId: "e1", ticketId: "t1", eventName: "Rock Festival 2026" })
        })
    ];

    renderNotificationDropdown();

    expect(getNotificationText(0)).toContain("Rock Festival 2026");
}

export function should_show_empty_state_when_no_notifications() {
    mockNotifications = [];

    renderNotificationDropdown();

    expect(emptyStateIsRendered()).toBe(true);
}

export function should_show_loading_state_while_fetching() {
    mockIsLoading = true;
    mockNotifications = [];

    renderNotificationDropdown();

    expect(loadingStateIsRendered()).toBe(true);
}

export function should_highlight_unread_notifications() {
    mockNotifications = [
        createNotification({ Id: "notif-1", IsRead: false }),
        createNotification({ Id: "notif-2", IsRead: true })
    ];

    renderNotificationDropdown();

    expect(isNotificationUnread(0)).toBe(true);
    expect(isNotificationUnread(1)).toBe(false);
}
