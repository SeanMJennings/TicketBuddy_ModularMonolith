import {
    Container,
    EventsManagementLink,
    HeaderBar,
    TicketStubImage,
    UserIcon,
    UserIconContainer,
} from "./Header.styles.tsx";
import * as React from "react";
import {UserType} from "../domain/user.ts";
import {useNavigate} from "react-router-dom";
import {useAuth} from "react-oidc-context";
import {convertToTicketBuddyUser} from "../oidc/key-cloak-user.extensions.ts";

export const Header = () => {
    const auth = useAuth();
    const user = convertToTicketBuddyUser(auth.user);
    const navigate = useNavigate();

    const onUserIconClick = (e: React.MouseEvent) => {
        e.stopPropagation();
        navigate('/profile');
    };

    return (
        <HeaderBar>
            <TicketStubImage/>
            <h1>TicketBuddy</h1>
            <Container>
                {user &&
                    <>
                        {user.UserType === UserType.Administrator && <EventsManagementLink to="/events-management">Events Management</EventsManagementLink>}
                        <UserIconContainer onClick={onUserIconClick} data-testid="user-icon">
                            <UserIcon />
                        </UserIconContainer>
                    </>
                }
                {
                    !auth.isAuthenticated ?
                    <button onClick={() => auth.signinRedirect()}>Login</button> :
                    <button onClick={() => auth.signoutRedirect()}>Logout</button>
                }
            </Container>
        </HeaderBar>
    );
}