import { render, type RenderResult } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { userEvent } from "@testing-library/user-event";
import { Main } from "../../app/App.tsx";

let renderedComponent: RenderResult;

export function renderVenuesManagement() {
    renderedComponent = render(
        <MemoryRouter initialEntries={["/venues-management"]}>
            <Main />
        </MemoryRouter>
    );
    return renderedComponent;
}

export function unmountVenuesManagement() {
    renderedComponent.unmount();
}

export function venueExists(venueName: string): boolean {
    return elements.theVenue(venueName) !== null;
}

export function clickAddVenueButton() {
    const button = elements.addVenueButton();
    return userEvent.click(button);
}

export function createVenueFormIsRendered() {
    return elements.createVenueForm() !== null;
}

export function updateVenueFormIsRendered() {
    return elements.updateVenueForm() !== null;
}

export async function fillVenueForm(venueData: {
    name: string;
    street: string;
    city: string;
    postCode: string;
    capacity: number;
}) {
    const nameField = elements.theFormField("Venue Name");
    await userEvent.clear(nameField);
    const streetField = elements.theFormField("Street");
    await userEvent.clear(streetField);
    const cityField = elements.theFormField("City");
    await userEvent.clear(cityField);
    const postCodeField = elements.theFormField("Post Code");
    await userEvent.clear(postCodeField);
    const capacityField = elements.theFormField("Capacity");
    await userEvent.clear(capacityField);

    await userEvent.type(elements.theFormField("Venue Name"), venueData.name);
    await userEvent.type(elements.theFormField("Street"), venueData.street);
    await userEvent.type(elements.theFormField("City"), venueData.city);
    await userEvent.type(elements.theFormField("Post Code"), venueData.postCode);
    await userEvent.type(elements.theFormField("Capacity"), venueData.capacity.toString());
}

export async function clickSubmitVenueButtonToCreate() {
    const createButton = elements.createVenueButton();
    return userEvent.click(createButton);
}

export async function clickSubmitVenueButtonToUpdate() {
    const updateButton = elements.updateVenueButton();
    return userEvent.click(updateButton);
}

export function backButtonIsRendered() {
    return elements.backButton() !== null;
}

export async function clickBackButton() {
    const button = elements.backButton();
    return userEvent.click(button);
}

export async function clickEditButtonForVenue(venueName: string) {
    const editButton = elements.editButtonForVenue(venueName);
    return userEvent.click(editButton);
}

export function editButtonExistsForVenue(venueName: string): boolean {
    const editButton = elements.editButtonForVenue(venueName);
    return !!editButton;
}

const elements = {
    theVenue: (venueName: string) => renderedComponent.queryByText(venueName),
    addVenueButton: () => renderedComponent.getByRole("link", { name: /add venue/i }),
    createVenueForm: () => renderedComponent.queryByTestId("venue-creation-form"),
    updateVenueForm: () => renderedComponent.queryByTestId("venue-update-form"),
    theFormField: (name: string) => renderedComponent.getByLabelText(name),
    createVenueButton: () => renderedComponent.getByRole("button", { name: /create venue/i }),
    updateVenueButton: () => renderedComponent.getByRole("button", { name: /update venue/i }),
    backButton: () => renderedComponent.getByRole("button", { name: /back to venues/i }),
    editButtonForVenue: (venueName: string) => {
        return renderedComponent.getByTestId(`edit-venue-${venueName}`);
    },
};
