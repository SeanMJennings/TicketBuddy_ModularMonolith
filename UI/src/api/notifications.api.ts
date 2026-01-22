import { get, post } from "../common/http";
import { NotificationSchema, UnreadCountSchema, type Notification, type UnreadCount } from "../domain/notification";
import { z } from "zod";

const NotificationsArraySchema = z.array(NotificationSchema);

export const getNotifications = async (jwt: string): Promise<Notification[]> => {
    const response = await get<unknown[]>("/notifications", jwt);
    return NotificationsArraySchema.parse(response);
};

export const markNotificationAsRead = async (id: string, jwt: string): Promise<void> => {
    await post(`/notifications/${id}/read`, {}, jwt);
};

export const getUnreadCount = async (jwt: string): Promise<UnreadCount> => {
    const response = await get<unknown>("/notifications/unread-count", jwt);
    return UnreadCountSchema.parse(response);
};
