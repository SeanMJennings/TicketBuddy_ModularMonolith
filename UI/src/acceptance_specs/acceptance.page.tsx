import {render, type RenderResult} from "@testing-library/react";
import App from "../app/App.tsx";
import {MemoryRouter} from "react-router-dom";
import {userEvent} from "@testing-library/user-event";

let renderedComponent: RenderResult;

export function renderApp() {
    renderedComponent = render(
        <MemoryRouter>
            <App/>
        </MemoryRouter>)
    return renderedComponent;
}

export function unmountApp() {
    renderedComponent.unmount();
}

export async function clickUsersDropdown() {
    const usersDropdown = await elements.theUsersDropdown();
    return userEvent.click(usersDropdown);
}

export async function selectUserFromDropdown(id: string) {
    const usersDropdown = await elements.theUsersDropdown();
    await userEvent.click(usersDropdown);
    return userEvent.selectOptions(usersDropdown, id);
}

export async function clickFirstEvent() {
    const link = await elements.firstEvent();
    return userEvent.click(link);
}

export function clickSeat(seatNumber: number) {
    return userEvent.click(elements.getSeatElement(seatNumber)!);
}

export function clickProceedToPurchaseButton() {
    return userEvent.click(elements.proceedToPurchaseButton()!);
}

export function purchaseButtonIsDisplayed() {
    return elements.purchaseButton() !== null;
}

export function clickPurchaseButton() {
    return userEvent.click(elements.purchaseButton()!);
}

export function clickBackToEventsButton() {
    return userEvent.click(elements.backToEventsButton()!);
}

export async function clickUserIcon() {
    const theUserIcon = await elements.theUserIcon();
    return userEvent.click(theUserIcon);
}

export function getTicketsList(): string[] {
    const elements = renderedComponent.container.querySelectorAll('[data-testid="ticket-item"]');
    return Array.from(elements).map(element => element.textContent || '');
}

const elements = {
    home: () => renderedComponent.queryByText("I am the mocked Home component"),
    theUsersDropdown: () => renderedComponent.findByTestId("users-dropdown"),
    firstEvent: async () => (await renderedComponent.findAllByText("Find Tickets"))[0],
    getSeatElement: (seatNumber: number)=> renderedComponent.container.querySelector(`[data-seat="${seatNumber}"]`),
    proceedToPurchaseButton: () => renderedComponent.getByText('Proceed to Purchase'),
    purchaseButton: () => renderedComponent.queryByText('Complete Purchase'),
    backToEventsButton: () => renderedComponent.queryByText('Back to Events'),
    theUserIcon: () => renderedComponent.findByTestId("user-icon"),
}