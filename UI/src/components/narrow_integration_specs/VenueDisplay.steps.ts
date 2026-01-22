import { afterEach, expect } from "vitest";
import {
    renderVenueDisplay,
    unmountVenueDisplay,
    venueNameIsDisplayed,
    unknownVenueIsDisplayed,
    venueAddressIsDisplayed
} from "./VenueDisplay.page.tsx";
import { Venues } from "../../testing/data.ts";

afterEach(() => {
    unmountVenueDisplay();
});

export function should_display_venue_name_when_venue_exists() {
    renderVenueDisplay(Venues, Venues[0].Id);

    expect(venueNameIsDisplayed(Venues[0].Name)).toBe(true);
}

export function should_display_unknown_venue_when_venue_does_not_exist() {
    renderVenueDisplay(Venues, "non-existent-id");

    expect(unknownVenueIsDisplayed()).toBe(true);
}

export function should_display_unknown_venue_when_venues_array_is_empty() {
    renderVenueDisplay([], "any-id");

    expect(unknownVenueIsDisplayed()).toBe(true);
}

export function should_display_venue_name_for_different_venues() {
    renderVenueDisplay(Venues, Venues[1].Id);

    expect(venueNameIsDisplayed(Venues[1].Name)).toBe(true);
}

export function should_display_venue_address_when_venue_exists() {
    const venue = Venues[0];
    renderVenueDisplay(Venues, venue.Id);

    expect(venueAddressIsDisplayed(venue.Address.Street, venue.Address.City, venue.Address.Postcode)).toBe(true);
}
