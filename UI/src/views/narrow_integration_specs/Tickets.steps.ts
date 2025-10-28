import {MockServer} from "../../testing/mock-server.ts";
import {afterEach, beforeEach, expect} from "vitest";
import {
    renderTickets,
    getSeatElement,
    unmountTickets,
    getSeatRow,
    titleIsRendered,
    clickBackToEventsButton,
    homePageIsRendered,
    clickSeat,
    getSelectedSeats,
    clickProceedToPurchaseButton,
    purchasePageIsRendered, errorToastIsDisplayed
} from "./Tickets.page.tsx";
import {waitUntil} from "../../testing/utilities.ts";
import {Events, TicketsForFirstEvent, Users} from "../../testing/data.ts";
import { vi } from "vitest";

const mockServer = MockServer.New();
let wait_for_get_event: () => boolean;
let wait_for_get_tickets: () => boolean;
let wait_for_reserve_tickets: () => boolean;

vi.mock("../../stores/users.store", () => {
    return {
        useUsersStore: () => {
            return {
                user: Users[0],
            }
        }
    }
})

beforeEach(() => {
    mockServer.reset();
    wait_for_get_event = mockServer.get(`/events/${Events[0].Id}`, Events[0]);
    wait_for_get_tickets = mockServer.get(`/events/${Events[0].Id}/tickets`, TicketsForFirstEvent);
    wait_for_reserve_tickets = mockServer.post(`/events/${Events[0].Id}/tickets/reserve`, {});
    mockServer.start();
});

afterEach(() => {
    unmountTickets();
});

export async function should_display_seat_map_for_event() {
    renderTickets(Events[0].Id);
    await waitUntil(wait_for_get_event);
    await waitUntil(wait_for_get_tickets);

    for (let i = 1; i <= 10; i++) {
        const seat = getSeatElement(i);
        expect(seat).not.toBeNull();
    }

    expect(titleIsRendered(Events[0].EventName)).toBeTruthy();
    expect(getSeatRow(1)).not.toBeNull();
    expect(getSeatRow(2)).not.toBeNull();
}

export async function should_navigate_back_to_events() {
    renderTickets(Events[0].Id);
    await waitUntil(wait_for_get_event);
    await waitUntil(wait_for_get_tickets);
    await clickBackToEventsButton();
    expect(homePageIsRendered()).toBeTruthy();
}

export async function should_allow_selecting_multiple_seats_and_proceed_to_purchase() {
    renderTickets(Events[0].Id);
    await waitUntil(wait_for_get_event);
    await waitUntil(wait_for_get_tickets);

    await clickSeat(1);
    await clickSeat(3);
    await clickSeat(5);

    expect(getSelectedSeats().length).toBe(3);

    await clickProceedToPurchaseButton();
    await waitUntil(wait_for_reserve_tickets);
    await waitUntil(() => purchasePageIsRendered());
    expect(purchasePageIsRendered()).toBeTruthy();
}

export async function should_show_error_if_fail_to_reserve_ticket() {
    renderTickets(Events[0].Id);
    await waitUntil(wait_for_get_event);
    await waitUntil(wait_for_get_tickets);

    mockServer.reset();
    wait_for_reserve_tickets = mockServer.post(`/events/${Events[0].Id}/tickets/reserve`, { Errors: ["Tickets already reserved"] }, 422);
    mockServer.start();

    await clickSeat(1);
    await clickSeat(3);
    await clickSeat(5);

    expect(getSelectedSeats().length).toBe(3);

    await clickProceedToPurchaseButton();
    await waitUntil(wait_for_reserve_tickets);
    await waitUntil(() => errorToastIsDisplayed("Tickets already reserved"));
    expect(purchasePageIsRendered()).toBeFalsy();
}

export async function should_not_allow_selecting_a_purchased_ticket() {
    renderTickets(Events[0].Id);
    await waitUntil(wait_for_get_event);
    await waitUntil(wait_for_get_tickets);

    await clickSeat(10);
    expect(getSelectedSeats().length).toBe(0);
    await clickSeat(1);
    expect(getSelectedSeats().length).toBe(1);
}

export async function should_organize_selected_seats_in_ascending_order() {
    renderTickets(Events[0].Id);
    await waitUntil(wait_for_get_event);
    await waitUntil(wait_for_get_tickets);

    await clickSeat(5);
    await clickSeat(2);
    await clickSeat(8);
    await clickSeat(1);
    await clickSeat(3);

    const selectedSeats = getSelectedSeats();
    expect(selectedSeats).toEqual([1, 2, 3, 5, 8]);
}
