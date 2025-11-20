import {vi} from 'vitest';
import {MockServer} from "../../testing/mock-server.ts";
import {afterEach, beforeEach} from "vitest";
import {AnOidcCustomerUser, Events} from "../../testing/data.ts";
import {
    clickFindTicketsButton,
    eventExists, findTicketsButtonExists,
    renderHome,
    soldOutMessageExists,
    unmountHome
} from "./Home.page.tsx";
import {waitUntil} from "../../testing/utilities.ts";
import {expect} from "vitest";
import React from "react";
import type {OidcUser} from "../../domain/user.ts";

vi.resetModules();

const mockedUseNavigate = vi.fn();
const mockServer = MockServer.New();
let wait_for_get_events: () => boolean;
let isAuthenticated = true;
let user : OidcUser | null = AnOidcCustomerUser;

vi.mock("react-router-dom", async () => {
    const mod = await vi.importActual<typeof import("react-router-dom")>(
        "react-router-dom"
    );
    return {
        ...mod,
        useNavigate: () => mockedUseNavigate,
    };
});

vi.mock('react-oidc-context', () => {
    return {
        AuthProvider: ({ children }: { children?: React.ReactNode }) => {
            return React.createElement(React.Fragment, null, children);
        },
        useAuth: () => ({
            isAuthenticated: isAuthenticated,
            user: user,
            signinRedirect: async () => {},
            signoutRedirect: async () => {},
        }),
    };
});

beforeEach(() => {
    isAuthenticated = true;
    user = AnOidcCustomerUser;
    mockServer.reset();
    wait_for_get_events = mockServer.get("events", Events)
    mockServer.start();
});

afterEach(() => {
    unmountHome();
});

export async function should_load_events_on_render() {
    renderHome();
    await waitUntil(wait_for_get_events);
    for (const event of Events) {
        expect(eventExists(event.EventName)).toBeTruthy();
    }
}

export async function should_not_show_find_tickets_when_user_logged_out() {
    isAuthenticated = false;
    user = null;
    renderHome();
    await waitUntil(wait_for_get_events);
    expect(findTicketsButtonExists(0)).toBeFalsy();
}

export async function should_navigate_to_tickets_page_when_find_tickets_clicked() {
    renderHome();
    await waitUntil(wait_for_get_events);
    await clickFindTicketsButton(0);
    expect(mockedUseNavigate).toHaveBeenCalledWith("/tickets/1");
}

export async function should_show_sold_out_message_for_sold_out_events() {
    renderHome();
    await waitUntil(wait_for_get_events);
    expect(soldOutMessageExists("Football Match at Wembley Stadium")).toBeTruthy();
}

