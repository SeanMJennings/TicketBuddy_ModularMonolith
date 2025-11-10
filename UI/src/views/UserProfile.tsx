import {useEffect, useState} from 'react';
import {Link} from 'react-router-dom';
import {getTicketsForUser} from '../api/tickets.api';
import {getEvents} from '../api/events.api';
import {type Ticket} from '../domain/ticket';
import {type Event, ConvertVenueToString} from '../domain/event';
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
import {useAuth} from "react-oidc-context";
import {convertToTicketBuddyUser} from "../oidc/key-cloak-user.extensions.ts";

export const UserProfile = () => {
    const [tickets, setTickets] = useState<Ticket[]>([]);
    const [events, setEvents] = useState<Event[]>([]);
    const [loading, setLoading] = useState(true);

    const auth = useAuth();
    const user = convertToTicketBuddyUser(auth.user);

    useEffect(() => {
        if (!auth.user?.access_token) return;

        const user = convertToTicketBuddyUser(auth.user);
        if (!user) return;

        Promise.all([
            getTicketsForUser(user.Id, auth.user?.access_token),
            getEvents()
        ])
            .then(([ticketsData, eventsData]) => {
                setTickets(ticketsData);
                setEvents(eventsData);
                setLoading(false);
            })
            .catch(() => {
                setLoading(false);
            });
    }, [auth.user]);

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

    const getEventName = (eventId: string): string => {
        const event = events.find(e => e.Id === eventId);
        return event ? event.EventName : 'Unknown Event';
    };

    const getEventDate = (eventId: string): string => {
        const event = events.find(e => e.Id === eventId);
        if (!event) return '';

        return event.StartDate.format('DD MMM YYYY');
    };

    const compareEventsByDate = (eventA: Event, eventB: Event): number => {
        return eventA.StartDate.valueOf() - eventB.StartDate.valueOf();
    };

    const getEventVenueName = (eventId: string): string => {
        const event = events.find(e => e.Id === eventId);
        if (!event) return '';

        return ConvertVenueToString(event.Venue);
    };

    const compareTicketsByEventDateThenSeatNumber = (ticketA: Ticket, ticketB: Ticket): number => {
        const eventA = events.find(e => e.Id === ticketA.EventId);
        const eventB = events.find(e => e.Id === ticketB.EventId);

        if (!eventA && !eventB) return 0;
        if (!eventA) return 1;
        if (!eventB) return -1;

        const dateDiff = compareEventsByDate(eventA, eventB);
        if (dateDiff !== 0) {
            return dateDiff;
        }

        return ticketA.SeatNumber - ticketB.SeatNumber;
    };

    const getSortedTickets = (): Ticket[] => {
        return tickets
            .slice()
            .sort(compareTicketsByEventDateThenSeatNumber);
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
                        {getInitials(user.FullName ?? '')}
                    </UserAvatar>
                    <UserName data-testid="user-name">{user.FullName}</UserName>
                    <UserEmail data-testid="user-email">{user.Email}</UserEmail>
                </UserCard>

                {!loading && tickets.length > 0 && (
                    <StatsGrid>
                        <StatCard data-testid="stat-card">
                            <div className="stat-value">{tickets.length}</div>
                            <div className="stat-label">Tickets Owned</div>
                        </StatCard>
                        <StatCard data-testid="stat-card">
                            <div className="stat-value">{formatCurrency(calculateTotalSpent())}</div>
                            <div className="stat-label">Total Spent</div>
                        </StatCard>
                        <StatCard data-testid="stat-card">
                            <div className="stat-value">{formatCurrency(calculateTotalSpent() / tickets.length)}</div>
                            <div className="stat-label">Average Price</div>
                        </StatCard>
                    </StatsGrid>
                )}

                <SectionTitle>My Bookings</SectionTitle>

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
                        {getSortedTickets().map((ticket) => (
                            <TicketCard key={ticket.Id} data-testid="ticket-item">
                                <TicketHeader>
                                    <SeatNumber>Seat {ticket.SeatNumber}</SeatNumber>
                                    <TicketPrice>{formatCurrency(ticket.Price)}</TicketPrice>
                                </TicketHeader>
                                <TicketMeta>
                                    <TicketDetail>{getEventName(ticket.EventId)}</TicketDetail>
                                    <TicketDetail>{getEventDate(ticket.EventId)}</TicketDetail>
                                    <TicketDetail>{getEventVenueName(ticket.EventId)}</TicketDetail>
                                </TicketMeta>
                            </TicketCard>
                        ))}
                    </TicketsGrid>
                )}
            </ProfileContainer>
        </Container>
    );
};
