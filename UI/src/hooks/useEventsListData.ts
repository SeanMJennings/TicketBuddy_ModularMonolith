import { useState, useEffect } from "react";
import { getEvents } from "../api/events.api";
import { getVenues } from "../api/venues.api";
import { type Event } from "../domain/event";
import { type Venue } from "../domain/venue";

type UseEventsListDataResult = {
    events: Event[];
    venues: Venue[];
    loading: boolean;
};

export const useEventsListData = (): UseEventsListDataResult => {
    const [events, setEvents] = useState<Event[]>([]);
    const [venues, setVenues] = useState<Venue[]>([]);
    const [loading, setLoading] = useState(true);

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
    }, []);

    return {
        events,
        venues,
        loading
    };
};
