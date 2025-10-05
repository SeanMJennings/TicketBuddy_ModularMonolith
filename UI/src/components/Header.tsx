import {
    Container,
    EventsManagementLink,
    HeaderBar,
    TicketStubImage,
    UserIcon,
    UserIconContainer,
    UsersDropdown
} from "./Header.styles.tsx";
import {useUsersStore} from "../stores/users.store.ts";
import {useShallow} from "zustand/react/shallow";
import * as React from "react";
import {UserType} from "../domain/user.ts";
import {useNavigate} from "react-router-dom";

export const Header = () => {
    const { user, users } = useUsersStore(useShallow((state => ({
        user: state.user,
        users: state.users
    }))));

    const navigate = useNavigate();

    const onUserIconClick = (e: React.MouseEvent) => {
        e.stopPropagation();
        navigate('/profile');
    };

    const onUsersDropdownChange = (e: React.MouseEvent<HTMLSelectElement>) => {
        e.stopPropagation();
        const target = e.target as HTMLSelectElement;
        const selectedUserId = target.value;
        const selectedUser = users.find(user => user.Id === selectedUserId);
        if (selectedUser?.Id !== user?.Id) {
            useUsersStore.setState({ user: selectedUser });
        }
    }

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
                {users?.length > 0 && <UsersDropdown data-testid="users-dropdown" onClick={onUsersDropdownChange}>
                    {users.map(user => (
                        <option key={user.Id} value={user.Id}>
                            {user.FullName}
                        </option>
                    ))}
                </UsersDropdown>}
            </Container>
        </HeaderBar>
    );
}