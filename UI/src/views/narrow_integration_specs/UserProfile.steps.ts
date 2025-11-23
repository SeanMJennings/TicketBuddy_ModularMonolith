import { afterEach, beforeEach, expect, vi } from "vitest";
import React from "react";
import {MockServer} from "../../testing/mock-server.ts";
import {
    renderUserProfile,
    unmountUserProfile,
    getUserNameDisplay,
    getUserEmailDisplay,
    getTicketsList,
    clickBackToHomeButton,
    homePageIsRendered,
    loadingIsDisplayed,
    getStatsCards,
} from "./UserProfile.page.tsx";
import {waitUntil} from "../../testing/utilities.ts";
import {Events, OidcUsers, Users} from "../../testing/data.ts";
import {ConvertVenueToString, type Event} from "../../domain/event.ts"
import moment from "moment/moment";

vi.resetModules();

vi.mock('react-oidc-context', () => {
    return {
        AuthProvider: ({ children }: { children?: React.ReactNode }) => {
            return React.createElement(React.Fragment, null, children);
        },
        useAuth: () => ({
            isAuthenticated: true,
            user: OidcUsers[0],
            signinRedirect: async () => {},
            signoutRedirect: async () => {},
        }),
    };
});

const mockServer = MockServer.New();
let wait_for_get_user_tickets: () => boolean;

const userTickets = [
    {
        Id: "ticket-1",
        EventId: "1",
        Price: 50.00,
        SeatNumber: 15,
        Purchased: true
    },
    {
        Id: "ticket-2",
        EventId: "2",
        Price: 75.00,
        SeatNumber: 8,
        Purchased: true
    }
];

beforeEach(() => {
    mockServer.reset();
    wait_for_get_user_tickets = mockServer.get(`tickets/users/me`, userTickets);
    mockServer.get('events', []);
    mockServer.start();
});

afterEach(() => {
    unmountUserProfile();
});

export async function should_display_user_profile_information() {
    renderUserProfile();
    await waitUntil(wait_for_get_user_tickets);

    expect(getUserNameDisplay()).toBe(Users[0].FullName);
    expect(getUserEmailDisplay()).toBe(Users[0].Email);
}

export async function should_display_user_tickets() {
    renderUserProfile();
    await waitUntil(wait_for_get_user_tickets);

    const ticketsList = getTicketsList();
    expect(ticketsList).toHaveLength(2);
    expect(ticketsList[0]).toContain("Seat 15");
    expect(ticketsList[0]).toContain("£50.00");
    expect(ticketsList[1]).toContain("Seat 8");
    expect(ticketsList[1]).toContain("£75.00");
}

export async function should_navigate_back_to_home() {
    renderUserProfile();
    await waitUntil(wait_for_get_user_tickets);

    await clickBackToHomeButton();
    expect(homePageIsRendered()).toBeTruthy();
}

export async function should_show_loading_while_fetching_tickets() {
    renderUserProfile();

    expect(loadingIsDisplayed()).toBeTruthy();
    await waitUntil(wait_for_get_user_tickets);
    expect(loadingIsDisplayed()).toBeFalsy();
}

export async function should_display_user_stats_when_tickets_exist() {
    renderUserProfile();
    await waitUntil(wait_for_get_user_tickets);

    const statsCards = getStatsCards();
    expect(statsCards).toHaveLength(3);

    expect(statsCards[0]).toContain("2");
    expect(statsCards[0]).toContain("Tickets Owned");

    expect(statsCards[1]).toContain("£125.00");
    expect(statsCards[1]).toContain("Total Spent");

    expect(statsCards[2]).toContain("£62.50");
    expect(statsCards[2]).toContain("Average Price");
}

export async function should_not_display_stats_when_no_tickets() {
    mockServer.reset();
    mockServer.get('events', []);
    wait_for_get_user_tickets = mockServer.get(`tickets/users/me`, []);
    mockServer.start();

    renderUserProfile();
    await waitUntil(wait_for_get_user_tickets);

    const statsCards = getStatsCards();
    expect(statsCards).toHaveLength(0);
}

export async function should_display_event_names_in_tickets() {
    mockServer.reset();
    mockServer.get('events', []);
    wait_for_get_user_tickets = mockServer.get(`tickets/users/me`, userTickets);
    mockServer.get('events', Events);
    mockServer.start();

    renderUserProfile();
    await waitUntil(wait_for_get_user_tickets);

    const ticketsList = getTicketsList();
    expect(ticketsList).toHaveLength(2);

    expect(ticketsList[0]).toContain(Events[0].EventName);
    expect(ticketsList[1]).toContain(Events[1].EventName);
}

export async function should_display_event_date_and_venue_in_tickets() {
    mockServer.reset();
    mockServer.get('events', []);
    wait_for_get_user_tickets = mockServer.get(`tickets/users/me`, userTickets);
    mockServer.get('events', Events);
    mockServer.start();

    renderUserProfile();
    await waitUntil(wait_for_get_user_tickets);

    const ticketsList = getTicketsList();
    expect(ticketsList).toHaveLength(2);

    const getEventDate = (theEvent: Event): string => {

        const startDate =   typeof theEvent.StartDate === 'string'
            ? moment(theEvent.StartDate)
            : theEvent.StartDate;

        return startDate.format('DD MMM YYYY');
    };

    expect(ticketsList[0]).toContain(getEventDate(Events[0]));
    expect(ticketsList[0]).toContain(ConvertVenueToString(Events[0].Venue));

    expect(ticketsList[1]).toContain(getEventDate(Events[1]));
    expect(ticketsList[1]).toContain(ConvertVenueToString(Events[1].Venue));
}

export async function should_order_tickets_by_event_date_then_seat_number() {
    const userTicketsUnordered = [
        {
            Id: "ticket-1",
            EventId: "event-future",
            Price: 50.00,
            SeatNumber: 15,
            Purchased: true
        },
        {
            Id: "ticket-2",
            EventId: "event-tomorrow",
            Price: 75.00,
            SeatNumber: 8,
            Purchased: true
        },
        {
            Id: "ticket-3",
            EventId: "event-tomorrow",
            Price: 60.00,
            SeatNumber: 3,
            Purchased: true
        },
        {
            Id: "ticket-4",
            EventId: "event-future",
            Price: 40.00,
            SeatNumber: 10,
            Purchased: true
        }
    ];

    const events = [
        {
            Id: "event-tomorrow",
            EventName: "Tomorrow Concert",
            StartDate: "2024-12-06T19:00:00",
            EndDate: "2024-12-06T22:00:00",
            Venue: 0,
            Price: 50
        },
        {
            Id: "event-future",
            EventName: "Future Concert",
            StartDate: "2024-12-20T20:00:00",
            EndDate: "2024-12-20T23:00:00",
            Venue: 1,
            Price: 75
        }
    ];

    mockServer.reset();
    mockServer.get('events', []);
    wait_for_get_user_tickets = mockServer.get(`tickets/users/me`, userTicketsUnordered);
    mockServer.get('events', events);
    mockServer.start();

    renderUserProfile();
    await waitUntil(wait_for_get_user_tickets);

    const ticketsList = getTicketsList();
    expect(ticketsList).toHaveLength(4);

    expect(ticketsList[0]).toContain("Tomorrow Concert");
    expect(ticketsList[0]).toContain("Seat 3");

    expect(ticketsList[1]).toContain("Tomorrow Concert");
    expect(ticketsList[1]).toContain("Seat 8");

    expect(ticketsList[2]).toContain("Future Concert");
    expect(ticketsList[2]).toContain("Seat 10");

    expect(ticketsList[3]).toContain("Future Concert");
    expect(ticketsList[3]).toContain("Seat 15");
}
