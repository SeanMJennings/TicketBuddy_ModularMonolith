import { render, screen, type RenderResult } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { Main } from "../../app/App.tsx";

let renderedComponent: RenderResult;

export function renderUserProfile() {
    renderedComponent = render(
        <MemoryRouter initialEntries={['/profile']}>
            <Main />
        </MemoryRouter>);
    return renderedComponent;
}

export function unmountUserProfile() {
    renderedComponent?.unmount();
}

export function getBookingsSectionTitle(): HTMLElement | null {
    return screen.queryByText("My Bookings");
}

export function getTicketsList() {
    const elements = renderedComponent.container.querySelectorAll('[data-testid="ticket-item"]');
    return Array.from(elements).map(element => element.textContent);
}

export function getStatsCards() {
    const statCards = renderedComponent.container.querySelectorAll('[data-testid="stat-card"]');
    return Array.from(statCards).map(card => card.textContent);
}
