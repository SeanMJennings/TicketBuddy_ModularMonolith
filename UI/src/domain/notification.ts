import { z } from "zod";

export enum NotificationType {
    TicketPurchased = "TicketPurchased"
}

export const NotificationSchema = z.object({
    Id: z.string(),
    UserId: z.string(),
    Type: z.enum(NotificationType),
    Payload: z.string(),
    IsRead: z.boolean(),
    CreatedAt: z.string()
});

export const UnreadCountSchema = z.object({
    Count: z.number().int().min(0)
});

const TicketPurchasedPayloadSchema = z.object({
    eventId: z.string(),
    ticketId: z.string(),
    eventName: z.string()
});

export type Notification = z.infer<typeof NotificationSchema>;
export type UnreadCount = z.infer<typeof UnreadCountSchema>;

export type TicketPurchasedPayload = {
    type: "TicketPurchased";
    eventId: string;
    ticketId: string;
    eventName: string;
};

export type UnknownPayload = {
    type: "Unknown";
    originalType: string;
};

export type ParsedNotificationPayload = TicketPurchasedPayload | UnknownPayload;

export const parseNotificationPayload = (notification: Notification): ParsedNotificationPayload => {
    try {
        const parsed = JSON.parse(notification.Payload);

        if (notification.Type === NotificationType.TicketPurchased) {
            const result = TicketPurchasedPayloadSchema.safeParse(parsed);
            if (result.success) {
                return {
                    type: "TicketPurchased",
                    eventId: result.data.eventId,
                    ticketId: result.data.ticketId,
                    eventName: result.data.eventName
                };
            }
        }

        return { type: "Unknown", originalType: notification.Type };
    } catch {
        return { type: "Unknown", originalType: notification.Type };
    }
};
