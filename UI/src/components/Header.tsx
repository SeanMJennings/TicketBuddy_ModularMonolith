import { useState } from "react";
import {
    Container,
    EventsManagementLink,
    HeaderBar,
    NotificationBadge,
    NotificationBellContainer,
    NotificationBellIcon,
    NotificationBellWrapper,
    TicketStubImage,
    UserIcon,
    UserIconContainer,
} from "./Header.styles.tsx";
import { NotificationDropdown } from "./NotificationDropdown";
import {useNotificationCount} from "../hooks/useNotificationCount";
import * as React from "react";
import {UserType} from "../domain/user.ts";
import {useNavigate} from "react-router-dom";
import {useAuth} from "react-oidc-context";
import {convertToTicketBuddyUser} from "../oidc/key-cloak-user.extensions.ts";
import {Button} from "./Button.styles.tsx";

export const Header = () => {
    const auth = useAuth();
    const user = convertToTicketBuddyUser(auth.user);
    const navigate = useNavigate();
    const { count } = useNotificationCount();
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);

    const onUserIconClick = (e: React.MouseEvent) => {
        e.stopPropagation();
        navigate('/profile');
    };

    const toggleDropdown = () => {
        setIsDropdownOpen((prev) => !prev);
    };

    return (
        <HeaderBar>
            <TicketStubImage/>
            <h1>TicketBuddy</h1>
            <Container>
                {user &&
                    <>
                        {user.UserType === UserType.Administrator && (
                            <>
                                <EventsManagementLink to="/events-management">Events Management</EventsManagementLink>
                                <EventsManagementLink to="/venues-management">Venues Management</EventsManagementLink>
                            </>
                        )}
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
                        <UserIconContainer onClick={onUserIconClick} data-testid="user-icon">
                            <UserIcon />
                        </UserIconContainer>
                    </>
                }
                {
                    !auth.isAuthenticated ?
                    <Button onClick={() => auth.signinRedirect()}>Login</Button> :
                    <Button onClick={() => auth.signoutRedirect()}>Logout</Button>
                }
            </Container>
        </HeaderBar>
    );
}