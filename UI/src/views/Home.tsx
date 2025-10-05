import {useEffect, useState} from "react";
import {getEvents} from "../api/events.api";
import {ConvertVenueToString, type Event} from "../domain/event";
import {Container, EventItem, EventList, PageTitle} from "./Common.styles.tsx";
import moment from "moment";
import {Button} from "../components/Button.styles.tsx";
import {useNavigate} from "react-router-dom";
import {ContentLoading} from "../components/LoadingContainers.styles.tsx";

export const Home = () => {
    const [events, setEvents] = useState<Event[]>([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        getEvents().then(data => {
            setEvents(data);
            setLoading(false);
        }).catch(() => {
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
                                <p>{moment(event.StartDate).format('MMMM Do YYYY, h:mm A')} to {moment(event.EndDate).format('MMMM Do YYYY, h:mm A')}</p>
                                <p>Venue: {ConvertVenueToString(event.Venue)}</p>
                                {event.IsSoldOut ? (<span>Sold Out</span>) : (<Button onClick={() => handleFindTickets(event.Id)}>Find Tickets</Button>)}
                            </div>
                        </EventItem>
                    ))}
                </EventList>
            )}
        </Container>
    );
}