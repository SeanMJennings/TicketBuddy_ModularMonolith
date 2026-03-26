import { afterEach, beforeEach, expect } from "vitest";
import {
    renderTicketPurchase,
    renderTicketPurchaseWithNoState,
    unmountTicketPurchase,
    titleIsRendered,
    backButtonIsRendered,
    purchaseButtonIsRendered,
    clickBackButton,
    clickPurchaseButton,
    seatsAreDisplayed,
    totalIsDisplayed,
    ticketsPageIsRendered,
    noTicketsSelectedMessageIsDisplayed
} from "./TicketPurchase.page.tsx";
import { MockServer } from "../../testing/mock-server.ts";
import { waitUntil } from "../../testing/utilities.ts";
import {Events, OidcUsers, TicketsForFirstEvent, Venues} from "../../testing/data.ts";
import { vi } from "vitest";
import React from "react";

const mockServer = MockServer.New();
const event = Events[0];
const mockTickets = [
    TicketsForFirstEvent[0],
    TicketsForFirstEvent[1],
    TicketsForFirstEvent[2],
];

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

let wait_for_post_purchase: () => boolean;

beforeEach(() => {
  mockServer.reset();
  mockServer.get("venues", Venues);
  wait_for_post_purchase = mockServer.post(`/events/${event.Id}/tickets/purchase`, {});
  mockServer.start();
});

afterEach(() => {
  unmountTicketPurchase();
});

export async function should_display_purchase_summary() {
  renderTicketPurchase(event.Id, mockTickets, event);

  expect(titleIsRendered()).toBeTruthy();
  expect(backButtonIsRendered()).toBeTruthy();
  expect(purchaseButtonIsRendered()).toBeTruthy();
  expect(seatsAreDisplayed()).toBeTruthy();
  expect(totalIsDisplayed()).toBeTruthy();
}

export async function should_purchase_tickets_successfully() {
  renderTicketPurchase(event.Id, mockTickets, event);

  await clickPurchaseButton();
  await waitUntil(wait_for_post_purchase);
  expect(mockServer.content).toStrictEqual({ TicketIds: [TicketsForFirstEvent[0].Id, TicketsForFirstEvent[1].Id, TicketsForFirstEvent[2].Id] });
  expect(backButtonIsRendered()).toBeFalsy();
}

export async function should_navigate_back_to_seat_selection() {
  renderTicketPurchase(event.Id, mockTickets, event);
  await clickBackButton();
  expect(ticketsPageIsRendered()).toBeTruthy();
}

export function should_show_no_tickets_selected_when_navigating_directly() {
  renderTicketPurchase(event.Id);
  expect(noTicketsSelectedMessageIsDisplayed()).toBeTruthy();
  expect(purchaseButtonIsRendered()).toBeFalsy();
}

export function should_show_no_tickets_selected_when_no_location_state() {
  renderTicketPurchaseWithNoState(event.Id);
  expect(noTicketsSelectedMessageIsDisplayed()).toBeTruthy();
  expect(purchaseButtonIsRendered()).toBeFalsy();
}
