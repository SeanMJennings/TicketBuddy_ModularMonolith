import {get, post, put} from "../common/http.ts";
import {type Event, type EventPayload, type UpdateEventPayload} from "../domain/event";

export const getEvents = async () => {
  return get<Event[]>("/events");
};

export const getEventById = async (id: string) => {
    return get<Event>(`/events/${id}`);
}

export const postEvent = async (event: EventPayload, jwt: string) => {
    return post("/events", event, jwt);
}

export const putEvent = async (id: string, event: UpdateEventPayload, jwt: string) => {
    return put(`/events/${id}`, event, jwt);
}