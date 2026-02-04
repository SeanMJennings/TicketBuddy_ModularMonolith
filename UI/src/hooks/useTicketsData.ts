import { useState, useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { getEventById } from "../api/events.api";
import { getTicketsForEvent } from "../api/tickets.api";
import { type Ticket } from "../domain/ticket";
import { type Event } from "../domain/event";

type UseTicketsDataResult = {
    tickets: Ticket[];
    event: Event | null;
    loading: boolean;
};

export const useTicketsData = (eventId: string | undefined): UseTicketsDataResult => {
    const [tickets, setTickets] = useState<Ticket[]>([]);
    const [event, setEvent] = useState<Event | null>(null);
    const [loading, setLoading] = useState(true);

    const auth = useAuth();

    useEffect(() => {
        const fetchEventAndTickets = async () => {
            await Promise.all([
                getEventById(eventId!),
                getTicketsForEvent(eventId!, auth.user?.access_token ?? '')
            ]).then(data => {
                setEvent(data[0]);
                setTickets(data[1]);
                setLoading(false);
            }).catch(() => {
                setLoading(false);
            });
        };

        fetchEventAndTickets();
    }, [auth.user?.access_token, eventId]);

    return {
        tickets,
        event,
        loading
    };
};
