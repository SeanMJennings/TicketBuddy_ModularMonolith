import {useEffect, useState} from 'react';
import {Link} from 'react-router-dom';
import {getTicketsForUser} from '../api/tickets.api';
import {type Ticket} from '../domain/ticket';
import {useUsersStore} from '../stores/users.store';
import {useShallow} from 'zustand/react/shallow';
import {Container, PageTitle} from './Common.styles';
import {ContentLoading} from '../components/LoadingContainers.styles';
import {Button} from '../components/Button.styles';
import {BackIcon} from './EventsManagement.styles';

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
                <p>No user selected</p>
            </Container>
        );
    }

    return (
        <Container>
            <PageTitle>User Profile</PageTitle>
            <Link to="/">
                <Button>
                    <BackIcon /> Back to Home
                </Button>
            </Link>

            <div>
                <h2 data-testid="user-name">{user.FullName}</h2>
                <p data-testid="user-email">{user.Email}</p>
            </div>

            <h3>My Tickets</h3>
            {loading ? (
                <ContentLoading/>
            ) : (
                <div>
                    {tickets.length === 0 ? (
                        <p>No tickets purchased yet</p>
                    ) : (
                        tickets.map((ticket) => (
                            <div key={ticket.Id} data-testid="ticket-item">
                                Seat {ticket.SeatNumber} - £{ticket.Price.toFixed(2)}
                            </div>
                        ))
                    )}
                </div>
            )}
        </Container>
    );
};
