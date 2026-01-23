import { useState, useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { getTicketsForUser } from "../api/tickets.api";
import { getEvents } from "../api/events.api";
import { getVenues } from "../api/venues.api";
import { type Ticket } from "../domain/ticket";
import { type Event } from "../domain/event";
import { type Venue } from "../domain/venue";
import { convertToTicketBuddyUser } from "../oidc/key-cloak-user.extensions";
import { type User, UserType } from "../domain/user";

type UseUserProfileDataResult = {
    tickets: Ticket[];
    events: Event[];
    venues: Venue[];
    loading: boolean;
    isAdmin: boolean;
    user: User | null;
};

export const useUserProfileData = (): UseUserProfileDataResult => {
    const [tickets, setTickets] = useState<Ticket[]>([]);
    const [events, setEvents] = useState<Event[]>([]);
    const [venues, setVenues] = useState<Venue[]>([]);
    const [loading, setLoading] = useState(true);

    const auth = useAuth();
    const user = convertToTicketBuddyUser(auth.user);
    const isAdmin = user?.UserType === UserType.Administrator;

    useEffect(() => {
        if (!auth.user?.access_token) return;

        const currentUser = convertToTicketBuddyUser(auth.user);
        if (!currentUser) return;

        if (currentUser.UserType === UserType.Administrator) {
            getVenues()
                .then(setVenues)
                .finally(() => setLoading(false));
            return;
        }

        Promise.all([
            getTicketsForUser(auth.user?.access_token),
            getEvents(),
            getVenues()
        ])
            .then(([ticketsData, eventsData, venuesData]) => {
                setTickets(ticketsData);
                setEvents(eventsData);
                setVenues(venuesData);
                setLoading(false);
            })
            .catch(() => {
                setLoading(false);
            });
    }, [auth.user]);

    return {
        tickets,
        events,
        venues,
        loading,
        isAdmin,
        user
    };
};
