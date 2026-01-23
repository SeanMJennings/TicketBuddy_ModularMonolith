import { type Event } from "./event";
import { formatDate } from "./formatting";

export const findEventById = (events: Event[], eventId: string): Event | undefined => {
    return events.find(e => e.Id === eventId);
};

export const getEventName = (events: Event[], eventId: string): string => {
    const event = findEventById(events, eventId);
    return event ? event.EventName : 'Unknown Event';
};

export const getEventDate = (events: Event[], eventId: string): string => {
    const event = findEventById(events, eventId);
    if (!event) return '';
    return formatDate(event.StartDate);
};

export const getEventVenueId = (events: Event[], eventId: string): string => {
    const event = findEventById(events, eventId);
    return event?.VenueId ?? '';
};

export const compareEventsByDate = (eventA: Event, eventB: Event): number => {
    return new Date(eventA.StartDate).valueOf() - new Date(eventB.StartDate).valueOf();
};
