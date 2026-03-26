import {render, type RenderResult, screen} from "@testing-library/react";
import {MemoryRouter} from "react-router-dom";
import {userEvent} from "@testing-library/user-event";
import {Main} from "../../app/App.tsx";

let renderedComponent: RenderResult;

export function renderEventsManagement() {
    renderedComponent = render(
        <MemoryRouter initialEntries={['/events-management']}>
            <Main/>
        </MemoryRouter>);
    return renderedComponent;
}

export function unmountEventsManagement() {
    renderedComponent.unmount();
}

export function eventExists(eventName: string): boolean {
    return elements.theEvent(eventName) !== null;
}

export function clickAddEventIcon() {
    const icon = elements.addEventIcon();
    return userEvent.click(icon);
}

export function createEventFormIsRendered() {
    return elements.createEventForm() !== null;
}

export function updateEventFormIsRendered() {
    return elements.updateEventForm() !== null;
}

export function formFieldIsRendered(fieldName: string) {
    return elements.formField(fieldName) !== null;
}

export function venueFieldIsDisabled() {
    const venueSelect = elements.venueField();
    return venueSelect ? venueSelect.disabled : false;
}

export async function fillEventForm(eventData: {
    eventName: string,
    startDate: string,
    endDate: string,
    venueId: string,
    Price: number
}) {
    const eventNameField = elements.theFormField("Event Name");
    await userEvent.clear(eventNameField);
    const startDateField = elements.theFormField("Start Date");
    await userEvent.clear(startDateField);
    const endDateField = elements.theFormField("End Date");
    await userEvent.clear(endDateField);
    const priceField = elements.theFormField("Ticket Price (£)");
    await userEvent.clear(priceField);
    await userEvent.type(elements.theFormField("Event Name"), eventData.eventName);
    await userEvent.type(elements.theFormField("Start Date"), eventData.startDate);
    await userEvent.type(elements.theFormField("End Date"), eventData.endDate);
    await userEvent.type(elements.theFormField("Ticket Price (£)"), eventData.Price.toString());

    const venueSelect = elements.venueField();
    if (venueSelect && !venueSelect.disabled) {
        await userEvent.selectOptions(venueSelect, eventData.venueId);
    }
}

export async function clickSubmitEventButtonToAddEvent() {
    const createButton = elements.createEventButton();
    return userEvent.click(createButton);
}

export async function clickSubmitEventButtonToUpdateEvent() {
    const updateButton = elements.updateEventButton();
    return userEvent.click(updateButton);
}

export function backButtonIsRendered() {
    return elements.backButton() !== null;
}

export async function clickBackButton() {
    const button = elements.backButton();
    return userEvent.click(button);
}

export async function clickEditButtonForEvent(eventName: string) {
    const editButton = elements.editButtonForEvent(eventName);
    return userEvent.click(editButton);
}

export function editButtonExistsForEvent(eventName: string): boolean {
    const editButton = elements.editButtonForEvent(eventName);
    return !!editButton;
}

export function loadingIsDisplayed(): boolean {
    return renderedComponent.container.querySelector('[data-testid="loading-indicator"]') !== null;
}

export function noEventsAreDisplayed(): boolean {
    return renderedComponent.container.querySelectorAll('.event-item').length === 0;
}

export function errorToastIsDisplayed(errorMessage: string): boolean {
    const toastElement = screen.getByText(errorMessage);
    return !!toastElement;
}

const elements = {
    theEvent: (eventName: string) => renderedComponent.getByText(eventName),
    addEventIcon: () => renderedComponent.getByRole("link", { name: /add event/i }),
    createEventForm: () => renderedComponent.queryByTestId("event-creation-form"),
    updateEventForm: () => renderedComponent.queryByTestId("event-update-form"),
    formField: (name: string) => renderedComponent.queryByLabelText(name),
    theFormField: (name: string) => renderedComponent.getByLabelText(name),
    venueField: () => renderedComponent.container.querySelector('#venueId') as HTMLSelectElement,
    createEventButton: () => renderedComponent.getByRole("button", { name: /create event/i }),
    updateEventButton: () => renderedComponent.getByRole("button", { name: /update event/i }),
    backButton: () => renderedComponent.getByRole("button", { name: /back to events/i }),
    editButtonForEvent: (eventName: string) => {
        return renderedComponent.getByTestId(`edit-event-${eventName}`);
    },
}
