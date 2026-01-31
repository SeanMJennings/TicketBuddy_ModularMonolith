import { useState } from "react";
import {
    NotificationBadge,
    NotificationBellContainer,
    NotificationBellIcon,
    NotificationBellWrapper,
} from "./Header.styles.tsx";
import { NotificationDropdown } from "./NotificationDropdown";
import {useNotificationCount} from "../hooks/useNotificationCount";

export const Notification = () => {
    const { count } = useNotificationCount();
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);

    const toggleDropdown = () => {
        setIsDropdownOpen((prev) => !prev);
    };

    return (
        <NotificationBellWrapper>
            <NotificationBellContainer
                data-testid="notification-bell"
                onClick={toggleDropdown}
            >
                <NotificationBellIcon />
                {count !== null && count > 0 && (
                    <NotificationBadge data-testid="notification-badge">
                        {count}
                    </NotificationBadge>
                )}
            </NotificationBellContainer>
            {isDropdownOpen && (
                <div data-testid="notification-dropdown">
                    <NotificationDropdown />
                </div>
            )}
        </NotificationBellWrapper>
    );
}