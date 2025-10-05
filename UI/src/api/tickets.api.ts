import {get, post} from "../common/http.ts";
import {type Ticket} from "../domain/ticket";

export const getTicketsForEvent = async (eventId: string) => {
    return get<Ticket[]>(`/events/${eventId}/tickets`);
}

export type TicketsPayload = {
    UserId: string;
    TicketIds: string[];
};

export const purchaseTickets = async (eventId: string, payload: TicketsPayload) => {
    return post(`/events/${eventId}/tickets/purchase`, payload);
}

export const reserveTickets = async (eventId: string, payload: TicketsPayload) => {
    return post(`/events/${eventId}/tickets/reserve`, payload);
}

export const getTicketsForUser = async (userId: string) => {
    return get<Ticket[]>(`/tickets/users/${userId}`);
}