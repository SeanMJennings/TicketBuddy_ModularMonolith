import {Link} from 'react-router-dom';
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
import {VenueDisplay} from "../components/VenueDisplay";
import {formatCurrency, getInitials} from "../domain/formatting";
import {getEventName, getEventDate, getEventVenueId} from "../domain/event.utils";
import {calculateTotalSpent, sortTicketsByEventDateThenSeatNumber} from "../domain/ticket.utils";
import {useUserProfileData} from "../hooks/useUserProfileData";

export const UserProfile = () => {
    const { tickets, events, venues, loading, isAdmin, user } = useUserProfileData();

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

    const sortedTickets = sortTicketsByEventDateThenSeatNumber(tickets, events);
    const totalSpent = calculateTotalSpent(tickets);

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

                {!isAdmin && (
                    <>
                        {!loading && tickets.length > 0 && (
                            <StatsGrid>
                                <StatCard data-testid="stat-card">
                                    <div className="stat-value">{tickets.length}</div>
                                    <div className="stat-label">Tickets Owned</div>
                                </StatCard>
                                <StatCard data-testid="stat-card">
                                    <div className="stat-value">{formatCurrency(totalSpent)}</div>
                                    <div className="stat-label">Total Spent</div>
                                </StatCard>
                                <StatCard data-testid="stat-card">
                                    <div className="stat-value">{formatCurrency(totalSpent / tickets.length)}</div>
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
                                {sortedTickets.map((ticket) => (
                                    <TicketCard key={ticket.Id} data-testid="ticket-item">
                                        <TicketHeader>
                                            <SeatNumber>Seat {ticket.SeatNumber}</SeatNumber>
                                            <TicketPrice>{formatCurrency(ticket.Price)}</TicketPrice>
                                        </TicketHeader>
                                        <TicketMeta>
                                            <TicketDetail>{getEventName(events, ticket.EventId)}</TicketDetail>
                                            <TicketDetail>{getEventDate(events, ticket.EventId)}</TicketDetail>
                                            <TicketDetail><VenueDisplay venues={venues} venueId={getEventVenueId(events, ticket.EventId)} /></TicketDetail>
                                        </TicketMeta>
                                    </TicketCard>
                                ))}
                            </TicketsGrid>
                        )}
                    </>
                )}
            </ProfileContainer>
        </Container>
    );
};
