import {useEffect, useState} from "react";
import {getEvents} from "../api/events.api";
import {type Event} from "../domain/event";
import {type Venue} from "../domain/venue";
import {getVenues} from "../api/venues.api";
import {Container, EventItem, EventList, PageTitle} from "./Common.styles.tsx";
import {Button} from "../components/Button.styles.tsx";
import {useNavigate} from "react-router-dom";
import {ContentLoading} from "../components/LoadingContainers.styles.tsx";
import {useAuth} from "react-oidc-context";
import {convertToTicketBuddyUser, isALoggedInCustomer} from "../oidc/key-cloak-user.extensions.ts";
import {VenueDisplay} from "../components/VenueDisplay.tsx";

export const Home = () => {
    const [events, setEvents] = useState<Event[]>([]);
    const [venues, setVenues] = useState<Venue[]>([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    const auth = useAuth();
    const user = convertToTicketBuddyUser(auth.user);

    useEffect(() => {
        Promise.all([getEvents(), getVenues()])
            .then(([eventsData, venuesData]) => {
                setEvents(eventsData);
                setVenues(venuesData);
                setLoading(false);
            })
            .catch(() => {
                setLoading(false);
            });
    },[]);

    const handleFindTickets = (eventId: string) => {
        navigate(`/tickets/${eventId}`);
    };

    return (
        <Container>
            <PageTitle>Upcoming Events</PageTitle>
            {loading ? (
                <ContentLoading />
            ) : (
                <EventList>
                    {events.map((event, index) => (
                        <EventItem key={index} data-testid="event-item">
                            <div>
                                <h2>{event.EventName}</h2>
                                <p>{event.StartDate.format('MMMM Do YYYY, h:mm A')} to {event.EndDate.format('MMMM Do YYYY, h:mm A')}</p>
                                <p>Venue: <VenueDisplay venues={venues} venueId={event.VenueId} /></p>
                                {isALoggedInCustomer(user) && (event.IsSoldOut ? (<span>Sold Out</span>) : (<Button onClick={() => handleFindTickets(event.Id)}>Find Tickets</Button>))}
                            </div>
                        </EventItem>
                    ))}
                </EventList>
            )}
        </Container>
    );
}