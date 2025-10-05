import {MockServer} from "../../testing/mock-server.ts";
import {afterEach, beforeEach, expect} from "vitest";
import {
    renderUserProfile,
    unmountUserProfile,
    getUserNameDisplay,
    getUserEmailDisplay,
    getTicketsList,
    clickBackToHomeButton,
    homePageIsRendered,
    loadingIsDisplayed,
    getStatsCards
} from "./UserProfile.page.tsx";
import {waitUntil} from "../../testing/utilities.ts";
import {Events, Users} from "../../testing/data.ts";
import {ConvertVenueToString, type Event} from "../../domain/event.ts"
import { vi } from "vitest";
import moment from "moment/moment";

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

vi.mock("../../stores/users.store", () => {
    return {
        useUsersStore: () => {
            return {
                user: Users[0],
            }
        }
    }
});

beforeEach(() => {
    mockServer.reset();
    wait_for_get_user_tickets = mockServer.get(`tickets/users/${Users[0].Id}`, userTickets);
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
    wait_for_get_user_tickets = mockServer.get(`tickets/users/${Users[0].Id}`, []);
    mockServer.start();

    renderUserProfile();
    await waitUntil(wait_for_get_user_tickets);

    const statsCards = getStatsCards();
    expect(statsCards).toHaveLength(0);
}

export async function should_display_event_names_in_tickets() {
    mockServer.reset();
    wait_for_get_user_tickets = mockServer.get(`tickets/users/${Users[0].Id}`, userTickets);
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
    wait_for_get_user_tickets = mockServer.get(`tickets/users/${Users[0].Id}`, userTickets);
    mockServer.get('events', Events);
    mockServer.start();

    renderUserProfile();
    await waitUntil(wait_for_get_user_tickets);

    const ticketsList = getTicketsList();
    expect(ticketsList).toHaveLength(2);

    const getEventDate = (theEvent: Event): string => {

        const startDate = typeof theEvent.StartDate === 'string'
            ? moment(theEvent.StartDate)
            : theEvent.StartDate;

        return startDate.format('DD MMM YYYY');
    };

    expect(ticketsList[0]).toContain(getEventDate(Events[0]));
    expect(ticketsList[0]).toContain(ConvertVenueToString(Events[0].Venue));

    expect(ticketsList[1]).toContain(getEventDate(Events[1]));
    expect(ticketsList[1]).toContain(ConvertVenueToString(Events[1].Venue));
}
