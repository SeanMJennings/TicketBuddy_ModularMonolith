import {MockServer} from "../testing/mock-server.ts";
import {afterEach, beforeEach, expect} from "vitest";
import {userRoutes} from "../api/users.api.ts";
import {Events, TicketBoughtForFirstEvent, TicketsForFirstEvent, Users} from "../testing/data.ts";
import {waitUntil} from "../testing/utilities.ts";
import {
    clickBackToEventsButton,
    clickFirstEvent, clickProceedToPurchaseButton, clickPurchaseButton, clickSeat, clickUserIcon,
    clickUsersDropdown, getTicketsList, purchaseButtonIsDisplayed,
    renderApp,
    selectUserFromDropdown,
    unmountApp
} from "./acceptance.page.tsx";

const mockServer = MockServer.New();
let wait_for_get_user: () => boolean;
let wait_for_get_events: () => boolean;
let wait_for_get_event: () => boolean;
let wait_for_get_tickets: () => boolean;
let wait_for_post_reservation: () => boolean;
let wait_for_post_ticket: () => boolean;
let wait_for_get_user_tickets: () => boolean;

beforeEach(() => {
    mockServer.reset();
    wait_for_get_user = mockServer.get(userRoutes.users, Users);
    Users.forEach(user => {
        mockServer.get(`tickets/users/${user.Id}`, []);
    });
    wait_for_get_events = mockServer.get("events", Events);
    wait_for_get_event = mockServer.get(`events/${Events[0].Id}`, Events[0]);
    wait_for_get_tickets = mockServer.get(`events/${Events[0].Id}/tickets`, TicketsForFirstEvent);
    wait_for_post_reservation = mockServer.post(`events/${Events[0].Id}/tickets/reserve`, {}, true);
    wait_for_post_ticket = mockServer.post(`events/${Events[0].Id}/tickets/purchase`, {}, true);
    wait_for_get_user_tickets = mockServer.get(`tickets/users/${Users[1].Id}`, TicketBoughtForFirstEvent);
    mockServer.start();
});

afterEach(() => {
    unmountApp();
});

export async function should_allow_a_user_to_purchase_a_ticket() {
    renderApp();
    await waitUntil(wait_for_get_user);
    await waitUntil(wait_for_get_events);
    await clickUsersDropdown();
    await selectUserFromDropdown(Users[1].Id);
    await clickFirstEvent();
    await waitUntil(wait_for_get_event);
    await waitUntil(wait_for_get_tickets);
    await clickSeat(1);
    await clickProceedToPurchaseButton();
    await waitUntil(wait_for_post_reservation);
    await waitUntil(() => purchaseButtonIsDisplayed());
    await clickPurchaseButton();
    await waitUntil(wait_for_post_ticket);

    mockServer.reset();
    wait_for_get_events = mockServer.get("events", Events);
    wait_for_get_user_tickets = mockServer.get(`tickets/users/${Users[1].Id}`, TicketBoughtForFirstEvent);
    mockServer.start();

    await clickBackToEventsButton();
    await waitUntil(wait_for_get_events);
    await clickUserIcon();
    await waitUntil(wait_for_get_user_tickets);

    const ticketsList = getTicketsList();
    expect(ticketsList).toHaveLength(1);

    expect(ticketsList[0]).toContain(Events[0].EventName);
}