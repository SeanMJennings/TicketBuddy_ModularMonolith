import {
    clickEventsManagementLink,
    clickTicketLogo,
    clickUserIcon,
    eventsManagementLinkIsRendered,
    eventsManagementPageIsRendered,
    homePageIsRendered,
    notFoundIsRendered,
    renderApp,
    renderAppAtEventsManagement,
    renderAppAtUnknownRoute,
    ticketLogoIsRendered,
    unmountApp,
    userProfilePageIsRendered,
    userIconIsRendered,
    errorPageIsRendered,
    renderAppAtError,
    clickLoginButton,
    clickLogoutButton, renderAppAtTickets, ticketsIsRendered, renderAppAtProfile
} from "./app.page";
import {afterEach, beforeEach, expect, vi} from "vitest";
import {MockServer} from "../../testing/mock-server";
import {AnOidcAdminUser, AnOidcCustomerUser, OidcUsers} from "../../testing/data";
import React from "react";

const mockServer = MockServer.New();
let isAuthenticated = true;
let user: unknown = null
let signInWasCalled = false;
let signOutWasCalled = false;

vi.resetModules();
vi.mock('react-oidc-context', () => {
    return {
        AuthProvider: ({ children }: { children?: React.ReactNode }) => {
            return React.createElement(React.Fragment, null, children);
        },
        useAuth: () => ({
            isAuthenticated: isAuthenticated,
            user: user,
            signinRedirect: async () => {
                signInWasCalled = true;
            },
            signoutRedirect: async () => {
                signOutWasCalled = true;
            },
        }),
    };
});

beforeEach(() => {
    mockServer.reset();
    isAuthenticated = false;
    user = null;
    signInWasCalled = false;
    signOutWasCalled = false;
    mockServer.get("events", []);
    mockServer.get(`tickets/users/me`, []);
    mockServer.start();
});

afterEach(() => {
    unmountApp();
});

export function should_default_to_home_page() {
    renderApp();
    expect(homePageIsRendered()).toBeTruthy();
}

export function should_display_not_found_for_unknown_routes() {
    renderAppAtUnknownRoute()
    expect(notFoundIsRendered()).toBeTruthy();
}

export async function should_let_a_user_login() {
    isAuthenticated = false;
    renderApp();
    await clickLoginButton();
    expect(signInWasCalled).toBeTruthy();
}

export async function should_let_a_user_logout() {
    isAuthenticated = true;
    renderApp();
    await clickLogoutButton();
    expect(signOutWasCalled).toBeTruthy();
}

export async function should_show_user_icon_when_user_logged_in() {
    isAuthenticated = true;
    user = OidcUsers[0];
    renderApp();
    expect(userIconIsRendered()).toBeTruthy();
}

export async function should_show_user_details_when_user_icon_is_clicked() {
    isAuthenticated = true;
    user = OidcUsers[0];
    renderApp();
    await clickUserIcon();
    expect(userProfilePageIsRendered()).toBeTruthy();
}

export async function should_display_event_management_navigation_if_user_is_admin() {
    isAuthenticated = true;
    user = AnOidcAdminUser;
    renderApp();
    expect(eventsManagementLinkIsRendered()).toBeTruthy();
}

export async function should_navigate_to_events_management_page_when_link_is_clicked() {
    isAuthenticated = true;
    user = AnOidcAdminUser;
    renderApp();
    expect(eventsManagementLinkIsRendered()).toBeTruthy();
    await clickEventsManagementLink();
    expect(eventsManagementPageIsRendered()).toBeTruthy();
}

export async function should_navigate_to_home_page_when_ticket_logo_is_clicked() {
    isAuthenticated = true;
    user = AnOidcAdminUser
    renderAppAtEventsManagement();
    expect(eventsManagementPageIsRendered()).toBeTruthy();
    expect(ticketLogoIsRendered()).toBeTruthy();
    await clickTicketLogo();
    expect(homePageIsRendered()).toBeTruthy();
    expect(eventsManagementPageIsRendered()).toBeFalsy();
}

export async function should_redirect_to_error_page_on_server_error() {
    isAuthenticated = false;
    user = null
    renderAppAtError();
    expect(errorPageIsRendered()).toBeTruthy();
}

export async function should_not_navigate_to_events_management_page_for_non_admin() {
    isAuthenticated = true;
    user = AnOidcCustomerUser;
    renderAppAtEventsManagement();
    expect(eventsManagementPageIsRendered()).toBeFalsy();
}

export async function should_let_a_user_navigate_to_tickets() {
    isAuthenticated = true;
    user = AnOidcCustomerUser;
    renderAppAtTickets();
    expect(ticketsIsRendered()).toBeTruthy();
}

export async function should_not_let_non_customers_navigate_to_tickets() {
    isAuthenticated = true;
    user = AnOidcAdminUser;
    renderAppAtTickets();
    expect(ticketsIsRendered()).toBeFalsy();
}

export async function should_let_a_logged_in_user_navigate_to_profile() {
    isAuthenticated = true;
    user = AnOidcCustomerUser;
    renderAppAtProfile();
    expect(userProfilePageIsRendered()).toBeTruthy();
}

export async function should_not_let_a_logged_out_user_navigate_to_profile() {
    isAuthenticated = false;
    user = null
    renderAppAtProfile();
    expect(userProfilePageIsRendered()).toBeFalsy();
}