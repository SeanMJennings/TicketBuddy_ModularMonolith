import { useNotifications } from "../hooks/useNotifications";
import { parseNotificationPayload, type Notification } from "../domain/notification";
import {
    DropdownContainer,
    NotificationItem,
    NotificationTitle,
    NotificationTime,
    EmptyState,
    LoadingState
} from "./NotificationDropdown.styles";

const formatNotificationMessage = (notification: Notification): string => {
    const parsed = parseNotificationPayload(notification);

    if (parsed.type === "TicketPurchased") {
        return `You purchased a ticket for ${parsed.eventName}`;
    }

    return `Notification: ${parsed.originalType}`;
};

const formatTime = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString();
};

export const NotificationDropdown = () => {
    const { notifications, isLoading } = useNotifications();

    if (isLoading) {
        return (
            <DropdownContainer>
                <LoadingState data-testid="notifications-loading">
                    Loading notifications...
                </LoadingState>
            </DropdownContainer>
        );
    }

    if (notifications.length === 0) {
        return (
            <DropdownContainer>
                <EmptyState data-testid="notifications-empty">
                    No notifications
                </EmptyState>
            </DropdownContainer>
        );
    }

    return (
        <DropdownContainer>
            {notifications.map((notification) => (
                <NotificationItem
                    key={notification.Id}
                    $unread={!notification.IsRead}
                    data-testid="notification-item"
                    data-unread={!notification.IsRead}
                >
                    <NotificationTitle>
                        {formatNotificationMessage(notification)}
                    </NotificationTitle>
                    <NotificationTime>
                        {formatTime(notification.CreatedAt)}
                    </NotificationTime>
                </NotificationItem>
            ))}
        </DropdownContainer>
    );
};
