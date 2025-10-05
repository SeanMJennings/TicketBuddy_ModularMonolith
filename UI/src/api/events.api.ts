import {get, post, put} from "../common/http.ts";
import {type Event, type EventPayload, type UpdateEventPayload} from "../domain/event";
import moment from "moment/moment";

export const getEvents = async () => {
  return get<Event[]>("/events").then(
      (events) => events.map((event) => ({
          ...event,
          StartDate: moment(event.StartDate),
          EndDate: moment(event.EndDate),
      })));
};

export const getEventById = async (id: string) => {
    return get<Event>(`/events/${id}`).then(
        (event) => ({
            ...event,
            StartDate: moment(event.StartDate),
            EndDate: moment(event.EndDate),
        }));
}

export const postEvent = async (event: EventPayload) => {
    const eventWithMoments = {
        ...event,
        StartDate: event.StartDate.toISOString(),
        EndDate: event.EndDate.toISOString(),
    }
    return post("/events", eventWithMoments);
}

export const putEvent = async (id: string, event: UpdateEventPayload) => {
    const eventWithMoments = {
        ...event,
        StartDate: event.StartDate.toISOString(),
        EndDate: event.EndDate.toISOString(),
    }
    return put(`/events/${id}`, eventWithMoments);
}