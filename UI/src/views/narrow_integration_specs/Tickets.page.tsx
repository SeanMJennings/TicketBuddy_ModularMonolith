import {render, type RenderResult} from "@testing-library/react";
import {MemoryRouter} from "react-router-dom";
import {vi} from "vitest";
import {userEvent} from "@testing-library/user-event";
import {Main} from "../../App.tsx";

vi.mock("../Home", () => {
    return {
        Home: () => {
            return (
                <div>I am the mocked Home component</div>
            );
        }
    }
})

vi.mock("../TicketPurchase", () => {
    return {
        TicketPurchase: () => {
            return (
                <div data-testid="purchase-page">I am the mocked TicketPurchase component</div>
            );
        }
    }
})

let renderedComponent: RenderResult;

export function renderTickets(eventId: string) {
    renderedComponent = render(
        <MemoryRouter initialEntries={[`/tickets/${eventId}`]}>
            <Main/>
        </MemoryRouter>);
    return renderedComponent;
}

export function unmountTickets() {
    renderedComponent?.unmount();
}

export function titleIsRendered(eventName: string) {
    return elements.title(eventName) !== null;
}

export function getSeatElement(seatNumber: number) {
    return elements.getSeatElement(seatNumber);
}

export function getSeatRow(rowNumber: number) {
    return elements.getSeatRow(rowNumber);
}

export function clickBackToEventsButton() {
    return userEvent.click(elements.backToEventsButton()!);
}

export function homePageIsRendered() {
    return elements.homePageIsRendered() !== null;
}

export function clickSeat(seatNumber: number) {
    return userEvent.click(elements.getSeatElement(seatNumber)!);
}

export function getSelectedSeats() {
    return elements.getSelectedSeats();
}

export function clickProceedToPurchaseButton() {
    return userEvent.click(elements.proceedToPurchaseButton()!);
}

export function purchasePageIsRendered() {
    return elements.purchasePageIsRendered() !== null;
}

export function errorToastIsDisplayed(errorMessage: string): boolean {
    const toastElement = elements.toastElement(errorMessage);
    return !!toastElement;
}
export const elements = {
    title: (eventName: string) => renderedComponent.getByRole('heading', {name: `Tickets for Event: ${eventName}`}),
    getSeatElement: (seatNumber: number)=> renderedComponent.container.querySelector(`[data-seat="${seatNumber}"]`),
    getSeatRow: (rowNumber: number)=> renderedComponent.container.querySelector(`[data-row="${rowNumber}"]`),
    backToEventsButton: () => renderedComponent.getByText('Back to Events'),
    homePageIsRendered: () => renderedComponent.getByText('I am the mocked Home component'),
    proceedToPurchaseButton: () => renderedComponent.getByText('Proceed to Purchase'),
    getSelectedSeats: () => {
        const selectedElements = renderedComponent.container.querySelectorAll('.selected');
        return Array.from(selectedElements).map(element =>
            parseInt(element.getAttribute('data-seat') || '0', 10)
        ).filter(seatNumber => seatNumber > 0);
    },
    purchasePageIsRendered: () => renderedComponent.queryByTestId('purchase-page'),
    toastElement: (errorMessage: string) => renderedComponent.queryByText(errorMessage),
};
