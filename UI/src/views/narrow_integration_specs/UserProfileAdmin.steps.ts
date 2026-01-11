import { afterEach, beforeEach, expect, vi } from "vitest";
import React from "react";
import { MockServer } from "../../testing/mock-server.ts";
import {
    renderUserProfile,
    unmountUserProfile,
    getBookingsSectionTitle,
    getTicketsList,
    getStatsCards
} from "./UserProfileAdmin.page.tsx";
import { waitUntil } from "../../testing/utilities.ts";
import { AnOidcAdminUser, Venues } from "../../testing/data.ts";

vi.resetModules();

vi.mock('react-oidc-context', () => {
    return {
        AuthProvider: ({ children }: { children?: React.ReactNode }) => {
            return React.createElement(React.Fragment, null, children);
        },
        useAuth: () => ({
            isAuthenticated: true,
            user: AnOidcAdminUser,
            signinRedirect: async () => {},
            signoutRedirect: async () => {},
        }),
    };
});

const mockServer = MockServer.New();
let wait_for_get_venues: () => boolean;

beforeEach(() => {
    mockServer.reset();
    wait_for_get_venues = mockServer.get('venues', Venues);
    mockServer.start();
});

afterEach(() => {
    unmountUserProfile();
});

export async function should_not_display_bookings_section_for_admin_user() {
    renderUserProfile();
    await waitUntil(wait_for_get_venues);

    expect(getBookingsSectionTitle()).toBeNull();
    expect(getTicketsList()).toHaveLength(0);
    expect(getStatsCards()).toHaveLength(0);
}
