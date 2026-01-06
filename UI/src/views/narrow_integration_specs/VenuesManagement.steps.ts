import { MockServer } from "../../testing/mock-server.ts";
import { afterEach, beforeEach, expect } from "vitest";
import {
    renderVenuesManagement,
    unmountVenuesManagement,
    venueExists,
    clickAddVenueButton,
    createVenueFormIsRendered,
    fillVenueForm,
    clickSubmitVenueButtonToCreate,
    clickSubmitVenueButtonToUpdate,
    backButtonIsRendered,
    clickBackButton,
    clickEditButtonForVenue,
    editButtonExistsForVenue,
    updateVenueFormIsRendered
} from "./VenuesManagement.page.tsx";
import { waitUntil } from "../../testing/utilities.ts";
import { Venues } from "../../testing/data.ts";

const mockServer = MockServer.New();
let wait_for_post: () => boolean;
let wait_for_put: () => boolean;
let wait_for_get_venues: () => boolean;
let wait_for_get_venue: () => boolean;

beforeEach(() => {
    mockServer.reset();
    wait_for_get_venues = mockServer.get("/venues", Venues);
    wait_for_get_venue = mockServer.get(`/venues/${Venues[0].Id}`, Venues[0]);
    wait_for_post = mockServer.post("/venues", {});
    wait_for_put = mockServer.put(`/venues/${Venues[0].Id}`, {});
    mockServer.start();
});

afterEach(() => {
    unmountVenuesManagement();
});

export async function should_display_list_of_venues() {
    renderVenuesManagement();
    await waitUntil(wait_for_get_venues);
    for (const venue of Venues) {
        expect(venueExists(venue.Name)).toBeTruthy();
    }
}

export async function should_allow_user_to_create_new_venue() {
    renderVenuesManagement();
    await waitUntil(wait_for_get_venues);
    await clickAddVenueButton();
    expect(createVenueFormIsRendered()).toBeTruthy();

    const newVenue = {
        name: "Test Venue",
        street: "123 Test Street",
        city: "Test City",
        postCode: "T1 1TT",
        capacity: 30
    };

    await fillVenueForm(newVenue);
    await clickSubmitVenueButtonToCreate();
    await waitUntil(wait_for_post);

    const data = mockServer.content;
    expect(data).toEqual({
        Name: newVenue.name,
        Street: newVenue.street,
        City: newVenue.city,
        Postcode: newVenue.postCode,
        Capacity: 30
    });
}

export async function should_navigate_back_to_venues_list_when_back_button_is_clicked() {
    renderVenuesManagement();
    await waitUntil(wait_for_get_venues);
    await clickAddVenueButton();

    expect(createVenueFormIsRendered()).toBeTruthy();
    expect(backButtonIsRendered()).toBeTruthy();

    mockServer.reset();
    wait_for_get_venues = mockServer.get("/venues", Venues);
    mockServer.start();

    await clickBackButton();

    expect(createVenueFormIsRendered()).toBeFalsy();
    await waitUntil(wait_for_get_venues);

    for (const venue of Venues) {
        expect(venueExists(venue.Name)).toBeTruthy();
    }
}

export async function should_allow_user_to_edit_existing_venue() {
    renderVenuesManagement();
    await waitUntil(wait_for_get_venues);

    const venueToEdit = Venues[0];
    expect(editButtonExistsForVenue(venueToEdit.Name)).toBeTruthy();

    await clickEditButtonForVenue(venueToEdit.Name);
    await waitUntil(wait_for_get_venue);

    expect(updateVenueFormIsRendered()).toBeTruthy();

    const updatedVenue = {
        name: `${venueToEdit.Name} - Updated`,
        street: venueToEdit.Address.Street,
        city: venueToEdit.Address.City,
        postCode: venueToEdit.Address.Postcode,
        capacity: 35
    };

    await fillVenueForm(updatedVenue);
    await clickSubmitVenueButtonToUpdate();
    await waitUntil(wait_for_put);

    const data = mockServer.content;
    expect(data).toEqual({
        Name: updatedVenue.name,
        Street: updatedVenue.street,
        City: updatedVenue.city,
        Postcode: updatedVenue.postCode,
        Capacity: 35
    });

    await waitUntil(() => !updateVenueFormIsRendered());
    expect(updateVenueFormIsRendered()).toBeFalsy();
}
