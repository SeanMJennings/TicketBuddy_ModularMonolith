import {render, type RenderResult} from "@testing-library/react";
import {MemoryRouter} from "react-router-dom";
import App from "../../app/App.tsx";
import {userEvent} from "@testing-library/user-event";
import {initOptions, keycloak} from "../../oauth2/keycloak.ts";
import {ReactKeycloakProvider} from "@react-keycloak/web";

let renderedComponent: RenderResult;

export function renderHome() {
    renderedComponent = render(
        <ReactKeycloakProvider authClient={keycloak} initOptions={initOptions}>
            <MemoryRouter>
                <App/>
            </MemoryRouter>
        </ReactKeycloakProvider>)
    return renderedComponent;
}

export function unmountHome() {
    renderedComponent.unmount();
}

export function eventExists(eventName: string): boolean {
    return elements.theEvent(eventName) !== null;
}

export function soldOutMessageExists(eventName: string): boolean {
    const eventElement = elements.theEvent(eventName);
    if (!eventElement) return false;
    const eventContainer = eventElement.closest('[data-testid="event-item"]') || eventElement.parentElement;
    return eventContainer?.textContent?.includes('Sold Out') || false;
}

export function clickFindTicketsButton(index: number) {
    const buttons = elements.findTicketsButtons();
    return userEvent.click(buttons[index]);
}

const elements = {
    theEvent: (eventName: string) => renderedComponent.getByText(eventName),
    findTicketsButtons: () => renderedComponent.getAllByRole('button', {name: /Find Tickets/i}),
}