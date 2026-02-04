import {render, type RenderResult} from "@testing-library/react";
import {MemoryRouter} from "react-router-dom";
import App from "../../app/App.tsx";
import {userEvent} from "@testing-library/user-event";

let renderedComponent: RenderResult;

export function renderHome() {
    renderedComponent = render(
        <MemoryRouter>
            <App/>
        </MemoryRouter>)
    return renderedComponent;
}

export function unmountHome() {
    renderedComponent.unmount();
}

export function eventExists(eventName: string): boolean {
    return elements.theEvent(eventName) !== null;
}

export function soldOutMessageExists(eventName: string) {
    const eventElement = elements.theEvent(eventName);
    const eventContainer = eventElement.closest('[data-testid="event-item"]');
    return eventContainer?.textContent?.includes('Sold Out');
}

export function clickFindTicketsButton(index: number) {
    const buttons = elements.findTicketsButtons();
    return userEvent.click(buttons[index]);
}

export function findTicketsButtonExists(index: number): boolean {
    return elements.findTicketsButtonQuery(index) !== undefined;
}

const elements = {
    theEvent: (eventName: string) => renderedComponent.getByText(eventName),
    findTicketsButtons: () => renderedComponent.getAllByRole('button', {name: /Find Tickets/i}),
    findTicketsButtonQuery: (index: number) => renderedComponent.queryAllByRole('button', {name: /Find Tickets/i})[index],
}