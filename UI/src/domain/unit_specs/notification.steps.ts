import { expect } from "vitest";
import { NotificationSchema, UnreadCountSchema, parseNotificationPayload, type Notification } from "../notification";

const createNotification = (type: string, payload: string): Notification => ({
    Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
    UserId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    Type: type,
    Payload: payload,
    IsRead: false,
    CreatedAt: "2026-01-22T10:30:00+00:00"
});

export function should_parse_valid_notification() {
    const validNotification = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        UserId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Type: "TicketPurchased",
        Payload: '{"eventId": "b95e92a4-9893-4791-909f-1decf99ea8b4", "ticketId": "720effb3-1802-4bbe-b6c4-9415d43b936c", "eventName": "Jazz Evening"}',
        IsRead: false,
        CreatedAt: "2026-01-22T10:30:00+00:00"
    };

    const result = NotificationSchema.safeParse(validNotification);

    expect(result.success).toBe(true);
    expect(result.data?.Id).toBe(validNotification.Id);
    expect(result.data?.UserId).toBe(validNotification.UserId);
    expect(result.data?.Type).toBe(validNotification.Type);
    expect(result.data?.Payload).toBe(validNotification.Payload);
    expect(result.data?.IsRead).toBe(validNotification.IsRead);
    expect(result.data?.CreatedAt).toBe(validNotification.CreatedAt);
}

export function should_parse_read_notification() {
    const readNotification = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        UserId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Type: "TicketPurchased",
        Payload: '{"eventId": "b95e92a4-9893-4791-909f-1decf99ea8b4"}',
        IsRead: true,
        CreatedAt: "2026-01-22T10:30:00+00:00"
    };

    const result = NotificationSchema.safeParse(readNotification);

    expect(result.success).toBe(true);
    expect(result.data?.IsRead).toBe(true);
}

export function should_reject_notification_with_missing_fields() {
    const invalidNotification = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        Type: "TicketPurchased"
    };

    const result = NotificationSchema.safeParse(invalidNotification);

    expect(result.success).toBe(false);
}

export function should_reject_notification_with_invalid_isread_type() {
    const invalidNotification = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        UserId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Type: "TicketPurchased",
        Payload: "{}",
        IsRead: "false",
        CreatedAt: "2026-01-22T10:30:00+00:00"
    };

    const result = NotificationSchema.safeParse(invalidNotification);

    expect(result.success).toBe(false);
}

export function should_parse_valid_unread_count() {
    const validResponse = {
        Count: 5
    };

    const result = UnreadCountSchema.safeParse(validResponse);

    expect(result.success).toBe(true);
    if (result.success) {
        expect(result.data.Count).toBe(5);
    }
}

export function should_parse_zero_unread_count() {
    const validResponse = {
        Count: 0
    };

    const result = UnreadCountSchema.safeParse(validResponse);

    expect(result.success).toBe(true);
    expect(result.data?.Count).toBe(0);
}

export function should_reject_non_integer_count() {
    const invalidResponse = {
        Count: 5.5
    };

    const result = UnreadCountSchema.safeParse(invalidResponse);

    expect(result.success).toBe(false);
}

export function should_reject_negative_count() {
    const invalidResponse = {
        Count: -1
    };

    const result = UnreadCountSchema.safeParse(invalidResponse);

    expect(result.success).toBe(false);
}

export function should_parse_ticket_purchased_payload() {
    const notification = createNotification(
        "TicketPurchased",
        '{"eventId": "b95e92a4-9893-4791-909f-1decf99ea8b4", "ticketId": "720effb3-1802-4bbe-b6c4-9415d43b936c", "eventName": "Jazz Evening"}'
    );

    const result = parseNotificationPayload(notification);

    if (result.type === "TicketPurchased") {
        expect(result.eventId).toBe("b95e92a4-9893-4791-909f-1decf99ea8b4");
        expect(result.ticketId).toBe("720effb3-1802-4bbe-b6c4-9415d43b936c");
        expect(result.eventName).toBe("Jazz Evening");
    }

    expect(result.type).toBe("TicketPurchased");
}

export function should_return_unknown_for_unrecognized_type() {
    const notification = createNotification(
        "SomeNewType",
        '{"foo": "bar"}'
    );

    const result = parseNotificationPayload(notification);

    if (result.type === "Unknown") {
        expect(result.originalType).toBe("SomeNewType");
    }

    expect(result.type).toBe("Unknown");
}

export function should_return_unknown_for_invalid_json() {
    const notification = createNotification(
        "TicketPurchased",
        "not valid json"
    );

    const result = parseNotificationPayload(notification);

    expect(result.type).toBe("Unknown");
}

export function should_return_unknown_for_missing_required_payload_fields() {
    const notification = createNotification(
        "TicketPurchased",
        '{"eventId": "b95e92a4-9893-4791-909f-1decf99ea8b4"}'
    );

    const result = parseNotificationPayload(notification);

    expect(result.type).toBe("Unknown");
}
