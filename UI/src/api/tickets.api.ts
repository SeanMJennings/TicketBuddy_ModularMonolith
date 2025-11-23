import {get, post} from "../common/http.ts";
import {type Ticket} from "../domain/ticket";

export const getTicketsForEvent = async (eventId: string, jwt: string | null | undefined) => {
    return get<Ticket[]>(`/events/${eventId}/tickets`, jwt);
}

export type TicketsPayload = {
    TicketIds: string[];
};

export const purchaseTickets = async (eventId: string, payload: TicketsPayload, jwt: string | null | undefined) => {
    return post(`/events/${eventId}/tickets/purchase`, payload, jwt);
}

export const reserveTickets = async (eventId: string, payload: TicketsPayload, jwt: string | null | undefined) => {
    return post(`/events/${eventId}/tickets/reserve`, payload, jwt);
}

export const getTicketsForUser = async (jwt: string | null | undefined) => {
    return get<Ticket[]>(`/tickets/users/me`, jwt);
}