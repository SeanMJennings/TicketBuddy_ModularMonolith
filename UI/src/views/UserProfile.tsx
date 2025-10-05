import {useEffect, useState} from 'react';
import {Link} from 'react-router-dom';
import {getTicketsForUser} from '../api/tickets.api';
import {type Ticket} from '../domain/ticket';
import {useUsersStore} from '../stores/users.store';
import {useShallow} from 'zustand/react/shallow';
import {Container, PageTitle, ActionBar} from './Common.styles';
import {ContentLoading} from '../components/LoadingContainers.styles';
import {Button} from '../components/Button.styles';
import {BackIcon} from './EventsManagement.styles';
import {
    ProfileContainer,
    UserCard,
    UserAvatar,
    UserName,
    UserEmail,
    SectionTitle,
    TicketsGrid,
    TicketCard,
    TicketHeader,
    SeatNumber,
    TicketPrice,
    TicketMeta,
    TicketDetail,
    EmptyState,
    StatsGrid,
    StatCard
} from './UserProfile.styles';

export const UserProfile = () => {
    const [tickets, setTickets] = useState<Ticket[]>([]);
    const [loading, setLoading] = useState(true);

    const { user } = useUsersStore(useShallow((state) => ({
        user: state.user
    })));

    useEffect(() => {
        if (!user) return;

        getTicketsForUser(user.Id)
            .then(data => {
                setTickets(data);
                setLoading(false);
            })
            .catch(() => {
                setLoading(false);
            });
    }, [user]);

    if (!user) {
        return (
            <Container>
                <PageTitle>User Profile</PageTitle>
                <EmptyState>
                    <span className="emoji">👤</span>
                    <h4>No user selected</h4>
                    <p>Please select a user to view their profile</p>
                </EmptyState>
            </Container>
        );
    }

    const getInitials = (fullName: string): string => {
        return fullName
            .split(' ')
            .map(name => name.charAt(0))
            .join('')
            .toUpperCase()
            .slice(0, 2);
    };

    const calculateTotalSpent = (): number => {
        return tickets.reduce((total, ticket) => total + ticket.Price, 0);
    };

    const formatCurrency = (amount: number): string => {
        return `£${amount.toFixed(2)}`;
    };

    return (
        <Container>
            <ProfileContainer>
                <PageTitle>User Profile</PageTitle>
                <ActionBar>
                    <Link to="/">
                        <Button>
                            <BackIcon /> Back to Home
                        </Button>
                    </Link>
                </ActionBar>

                <UserCard>
                    <UserAvatar data-testid="user-avatar">
                        {getInitials(user.FullName)}
                    </UserAvatar>
                    <UserName data-testid="user-name">{user.FullName}</UserName>
                    <UserEmail data-testid="user-email">{user.Email}</UserEmail>
                </UserCard>

                {!loading && tickets.length > 0 && (
                    <StatsGrid>
                        <StatCard>
                            <div className="stat-value">{tickets.length}</div>
                            <div className="stat-label">Tickets Owned</div>
                        </StatCard>
                        <StatCard>
                            <div className="stat-value">{formatCurrency(calculateTotalSpent())}</div>
                            <div className="stat-label">Total Spent</div>
                        </StatCard>
                        <StatCard>
                            <div className="stat-value">{formatCurrency(calculateTotalSpent() / tickets.length)}</div>
                            <div className="stat-label">Average Price</div>
                        </StatCard>
                    </StatsGrid>
                )}

                <SectionTitle>My Tickets</SectionTitle>

                {loading ? (
                    <div className="loading-indicator">
                        <ContentLoading />
                    </div>
                ) : tickets.length === 0 ? (
                    <EmptyState>
                        <span className="emoji">🎫</span>
                        <h4>No tickets purchased yet</h4>
                        <p>Start exploring events to purchase your first tickets!</p>
                    </EmptyState>
                ) : (
                    <TicketsGrid>
                        {tickets.map((ticket) => (
                            <TicketCard key={ticket.Id} data-testid="ticket-item">
                                <TicketHeader>
                                    <SeatNumber>Seat {ticket.SeatNumber}</SeatNumber>
                                    <TicketPrice>{formatCurrency(ticket.Price)}</TicketPrice>
                                </TicketHeader>
                                <TicketMeta>
                                    <TicketDetail>Event ID: {ticket.EventId}</TicketDetail>
                                    <TicketDetail>Ticket ID: {ticket.Id}</TicketDetail>
                                    <TicketDetail>Status: Purchased</TicketDetail>
                                </TicketMeta>
                            </TicketCard>
                        ))}
                    </TicketsGrid>
                )}
            </ProfileContainer>
        </Container>
    );
};
