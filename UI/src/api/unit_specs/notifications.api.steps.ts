import { beforeEach, expect } from "vitest";
import { MockServer } from "../../testing/mock-server";
import { waitUntil } from "../../testing/utilities";
import { getNotifications, markNotificationAsRead, getUnreadCount } from "../notifications.api";
import { NotificationSchema } from "../../domain/notification";

const mockServer = MockServer.New();

const validNotificationResponse = {
    Id: "n1-uuid-0000-0000-000000000001",
    UserId: "u1-uuid-0000-0000-000000000001",
    Type: "TicketPurchased",
    Payload: '{"eventId": "e1", "ticketId": "t1", "eventName": "Jazz Evening"}',
    IsRead: false,
    CreatedAt: "2026-01-22T10:30:00+00:00"
};

const anotherNotificationResponse = {
    Id: "n2-uuid-0000-0000-000000000002",
    UserId: "u1-uuid-0000-0000-000000000001",
    Type: "TicketPurchased",
    Payload: '{"eventId": "e2", "ticketId": "t2", "eventName": "Rock Concert"}',
    IsRead: true,
    CreatedAt: "2026-01-21T14:00:00+00:00"
};

const jwt = "user-access-token";

beforeEach(() => {
    mockServer.reset();
    mockServer.start();
});

export async function should_fetch_all_notifications() {
    const wasCalled = mockServer.get("/notifications", [validNotificationResponse, anotherNotificationResponse]);

    const notifications = await getNotifications(jwt);

    await waitUntil(wasCalled);
    expect(notifications).toHaveLength(2);
    expect(notifications[0].Id).toBe("n1-uuid-0000-0000-000000000001");
    expect(notifications[1].Id).toBe("n2-uuid-0000-0000-000000000002");
}

export async function should_return_validated_notification_objects() {
    mockServer.get("/notifications", [validNotificationResponse]);

    const notifications = await getNotifications(jwt);

    const parseResult = NotificationSchema.safeParse(notifications[0]);
    expect(parseResult.success).toBe(true);
}

export async function should_pass_authorization_header_when_fetching_notifications() {
    const wasCalled = mockServer.get("/notifications", [validNotificationResponse]);

    await getNotifications(jwt);

    await waitUntil(wasCalled);
    expect(mockServer.headers.get("Authorization")).toBe(`Bearer ${jwt}`);
}

export async function should_return_empty_array_when_no_notifications() {
    mockServer.get("/notifications", []);

    const notifications = await getNotifications(jwt);

    expect(notifications).toHaveLength(0);
}

export async function should_mark_notification_as_read() {
    const notificationId = "n1-uuid-0000-0000-000000000001";
    const wasCalled = mockServer.post(`/notifications/${notificationId}/read`, undefined, 204);

    await markNotificationAsRead(notificationId, jwt);

    await waitUntil(wasCalled);
    expect(mockServer.headers.get("Authorization")).toBe(`Bearer ${jwt}`);
}

export async function should_fetch_unread_count() {
    const wasCalled = mockServer.get("/notifications/unread-count", { Count: 5 });

    const result = await getUnreadCount(jwt);

    await waitUntil(wasCalled);
    expect(result.Count).toBe(5);
}

export async function should_return_zero_unread_count() {
    mockServer.get("/notifications/unread-count", { Count: 0 });

    const result = await getUnreadCount(jwt);

    expect(result.Count).toBe(0);
}

export async function should_pass_authorization_header_when_fetching_unread_count() {
    const wasCalled = mockServer.get("/notifications/unread-count", { Count: 3 });

    await getUnreadCount(jwt);

    await waitUntil(wasCalled);
    expect(mockServer.headers.get("Authorization")).toBe(`Bearer ${jwt}`);
}
