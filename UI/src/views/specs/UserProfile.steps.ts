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
    loadingIsDisplayed
} from "./UserProfile.page.tsx";
import {waitUntil} from "../../testing/utilities.ts";
import {Users} from "../../testing/data.ts";
import { vi } from "vitest";

const mockServer = MockServer.New();
let wait_for_get_user_tickets: () => boolean;

const userTickets = [
    {
        Id: "ticket-1",
        EventId: "event-1",
        Price: 50.00,
        SeatNumber: 15,
        Purchased: true
    },
    {
        Id: "ticket-2",
        EventId: "event-2",
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
    wait_for_get_user_tickets = mockServer.get(`/tickets/users/${Users[0].Id}`, userTickets);
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
