import {MockServer} from "../../testing/mock-server.ts";
import {afterEach, beforeEach, expect} from "vitest";
import {
    createEventFormIsRendered,
    formFieldIsRendered,
    renderEventsManagement,
    unmountEventsManagement,
    fillEventForm,
    clickSubmitEventButtonToAddEvent,
    clickAddEventIcon,
    eventExists,
    backButtonIsRendered,
    clickBackButton,
    clickEditButtonForEvent,
    editButtonExistsForEvent,
    clickSubmitEventButtonToUpdateEvent,
    errorToastIsDisplayed,
    updateEventFormIsRendered,
    venueFieldIsDisabled
} from "./EventsManagement.page.tsx";
import {waitUntil} from "../../testing/utilities.ts";
import {Events, Venues} from "../../testing/data.ts";

const mockServer = MockServer.New();
let wait_for_post: () => boolean;
let wait_for_put: () => boolean;
let wait_for_get_events: () => boolean;
let wait_for_get_event: () => boolean;
let wait_for_get_venues: () => boolean;

beforeEach(() => {
    mockServer.reset();
    wait_for_get_events = mockServer.get("/events", Events);
    wait_for_get_event = mockServer.get(`/events/${Events[0].Id}`, Events[0]);
    wait_for_get_venues = mockServer.get("/venues", Venues);
    wait_for_post = mockServer.post("/events", {});
    wait_for_put = mockServer.put(`/events/${Events[0].Id}`, {});
    mockServer.start();
});

afterEach(() => {
    unmountEventsManagement();
});

export async function should_display_list_of_events() {
    renderEventsManagement();
    await waitUntil(wait_for_get_events);
    for (const event of Events) {
        expect(eventExists(event.EventName)).toBeTruthy();
    }
}

export async function should_render_event_creation_form() {
    renderEventsManagement();
    await clickAddEventIcon();
    expect(createEventFormIsRendered()).toBeTruthy();
    expect(formFieldIsRendered("Event Name")).toBeTruthy();
    expect(formFieldIsRendered("Start Date")).toBeTruthy();
    expect(formFieldIsRendered("End Date")).toBeTruthy();
    expect(formFieldIsRendered("Venue")).toBeTruthy();
    expect(formFieldIsRendered("Ticket Price (£)")).toBeTruthy();

}

export async function should_allow_user_to_create_new_event() {
    renderEventsManagement();
    await waitUntil(wait_for_get_venues);
    await clickAddEventIcon();
    expect(createEventFormIsRendered()).toBeTruthy();

    const eventName = "Test Event";

    const startEventDate = new Date();
    startEventDate.setDate(startEventDate.getDate() + 1);
    const startEventDateStringWithTime = startEventDate.toISOString().split("T")[0] + "T12:12";

    const endEventDate = new Date(startEventDate);
    endEventDate.setDate(endEventDate.getDate() + 2);
    const endEventDateStringWithTime = endEventDate.toISOString().split("T")[0] + "T13:13";
    const eventVenueId = Venues[0].Id;

    await fillEventForm({
        eventName: eventName,
        startDate: startEventDateStringWithTime,
        endDate: endEventDateStringWithTime,
        venueId: eventVenueId,
        Price: 45.00
    });
    await clickSubmitEventButtonToAddEvent();
    await waitUntil(wait_for_post);
    const data = mockServer.content
    expect(data).toEqual({
        EventName: eventName,
        StartDate: new Date(startEventDateStringWithTime).toISOString(),
        EndDate: new Date(endEventDateStringWithTime).toISOString(),
        VenueId: eventVenueId,
        Price: "45"
    });
}

export async function should_navigate_back_to_events_list_when_back_button_is_clicked() {
    renderEventsManagement();
    await waitUntil(wait_for_get_events);
    await waitUntil(wait_for_get_venues);
    await clickAddEventIcon();

    expect(createEventFormIsRendered()).toBeTruthy();
    expect(backButtonIsRendered()).toBeTruthy();

    mockServer.reset();
    wait_for_get_events = mockServer.get("/events", Events);
    wait_for_get_venues = mockServer.get("/venues", Venues);
    mockServer.start();

    await clickBackButton();

    expect(createEventFormIsRendered()).toBeFalsy();
    await waitUntil(wait_for_get_events);

    for (const event of Events) {
        expect(eventExists(event.EventName)).toBeTruthy();
    }
}

export async function should_allow_user_to_edit_existing_event() {
    renderEventsManagement();
    await waitUntil(wait_for_get_events);
    await waitUntil(wait_for_get_venues);

    const eventToEdit = Events[0];
    expect(editButtonExistsForEvent(eventToEdit.EventName)).toBeTruthy();

    await clickEditButtonForEvent(eventToEdit.EventName);
    await waitUntil(wait_for_get_event);

    const updatedEventName = `${eventToEdit.EventName} - Updated`;
    const startEventDate = new Date();
    startEventDate.setDate(startEventDate.getDate() + 7);
    const startEventDateStringWithTime = startEventDate.toISOString().split("T")[0] + "T14:00";

    const endEventDate = new Date(startEventDate);
    endEventDate.setDate(endEventDate.getDate() + 1);
    const endEventDateStringWithTime = endEventDate.toISOString().split("T")[0] + "T17:00";

    await fillEventForm({
        eventName: updatedEventName,
        startDate: startEventDateStringWithTime,
        endDate: endEventDateStringWithTime,
        venueId: eventToEdit.VenueId,
        Price: 50
    });

    await clickSubmitEventButtonToUpdateEvent();
    await waitUntil(wait_for_put);

    const data = mockServer.content;
    expect(data).toStrictEqual({
        EventName: updatedEventName,
        StartDate: new Date(startEventDateStringWithTime).toISOString(),
        EndDate: new Date(endEventDateStringWithTime).toISOString(),
        Price: "50"
    });

    await waitUntil(() => !updateEventFormIsRendered());
    expect(updateEventFormIsRendered()).toBeFalsy();
}

export async function should_show_error_toast_when_event_update_fails() {
    renderEventsManagement();
    await waitUntil(wait_for_get_events);
    await waitUntil(wait_for_get_venues);

    const eventToEdit = Events[0];
    expect(editButtonExistsForEvent(eventToEdit.EventName)).toBeTruthy();

    await clickEditButtonForEvent(eventToEdit.EventName);
    await waitUntil(wait_for_get_event);

    mockServer.reset();
    const errorResponse = {
        Message: "The request could not be correctly validated.",
        Errors: ["End date cannot be before start date"]
    };
    wait_for_get_venues = mockServer.get("/venues", Venues);
    wait_for_put = mockServer.put(`/events/${eventToEdit.Id}`, errorResponse, 422);
    mockServer.start();

    const currentDate = new Date();
    const startDate = new Date();
    startDate.setDate(currentDate.getDate() + 10);
    const endDate = new Date();
    endDate.setDate(currentDate.getDate() + 5);

    const startDateString = startDate.toISOString().split("T")[0] + "T14:00";
    const endDateString = endDate.toISOString().split("T")[0] + "T12:00";

    await fillEventForm({
        eventName: "Updated Event Name",
        startDate: startDateString,
        endDate: endDateString,
        venueId: eventToEdit.VenueId,
        Price: eventToEdit.Price
    });

    await clickSubmitEventButtonToUpdateEvent();
    await waitUntil(wait_for_put);

    expect(errorToastIsDisplayed("End date cannot be before start date")).toBeTruthy();
}

export async function should_show_error_toast_when_event_creation_fails() {
    renderEventsManagement();
    await waitUntil(wait_for_get_events);
    await waitUntil(wait_for_get_venues);

    await clickAddEventIcon();
    expect(createEventFormIsRendered()).toBeTruthy();

    mockServer.reset();
    const errorResponse = {
        Message: "The request could not be correctly validated.",
        Errors: ["End date cannot be before start date"]
    };
    wait_for_get_venues = mockServer.get("/venues", Venues);
    wait_for_post = mockServer.post("/events", errorResponse, 422, 400);
    mockServer.start();

    const currentDate = new Date();
    const startDate = new Date();
    startDate.setDate(currentDate.getDate() + 10);
    const endDate = new Date();
    endDate.setDate(currentDate.getDate() + 5);

    const startDateString = startDate.toISOString().split("T")[0] + "T14:00";
    const endDateString = endDate.toISOString().split("T")[0] + "T12:00";

    await fillEventForm({
        eventName: "Invalid Event",
        startDate: startDateString,
        endDate: endDateString,
        venueId: Venues[0].Id,
        Price: 30
    });

    await clickSubmitEventButtonToAddEvent();
    await waitUntil(wait_for_post);

    expect(errorToastIsDisplayed("End date cannot be before start date")).toBeTruthy();
}

export async function should_navigate_back_when_event_fetch_fails_in_edit_mode() {
    renderEventsManagement();
    await waitUntil(wait_for_get_events);
    await waitUntil(wait_for_get_venues);

    mockServer.reset();
    wait_for_get_event = mockServer.get(`/events/${Events[0].Id}`, { error: 'Not Found' }, undefined, 404);
    mockServer.start();

    await clickEditButtonForEvent(Events[0].EventName);
    await waitUntil(wait_for_get_event);
    expect(errorToastIsDisplayed("Failed to fetch event details")).toBeTruthy();
}

export async function should_not_allow_venue_change_when_editing_event() {
    renderEventsManagement();
    await waitUntil(wait_for_get_events);
    await waitUntil(wait_for_get_venues);

    const eventToEdit = Events[0];
    expect(editButtonExistsForEvent(eventToEdit.EventName)).toBeTruthy();

    await clickEditButtonForEvent(eventToEdit.EventName);
    await waitUntil(wait_for_get_event);

    expect(formFieldIsRendered("Venue")).toBeTruthy();
    expect(venueFieldIsDisabled()).toBeTruthy();

    const updatedEventName = `${eventToEdit.EventName} - Updated`;
    const startEventDate = new Date();
    startEventDate.setDate(startEventDate.getDate() + 7);
    const startEventDateStringWithTime = startEventDate.toISOString().split("T")[0] + "T14:00";

    const endEventDate = new Date(startEventDate);
    endEventDate.setDate(endEventDate.getDate() + 1);
    const endEventDateStringWithTime = endEventDate.toISOString().split("T")[0] + "T17:00";

    await fillEventForm({
        eventName: updatedEventName,
        startDate: startEventDateStringWithTime,
        endDate: endEventDateStringWithTime,
        venueId: eventToEdit.VenueId,
        Price: 50
    });

    await clickSubmitEventButtonToUpdateEvent();
    await waitUntil(wait_for_put);

    const data = mockServer.content;

    expect(data).toStrictEqual({
        EventName: updatedEventName,
        StartDate: new Date(startEventDateStringWithTime).toISOString(),
        EndDate: new Date(endEventDateStringWithTime).toISOString(),
        Price: "50"
    });
}
