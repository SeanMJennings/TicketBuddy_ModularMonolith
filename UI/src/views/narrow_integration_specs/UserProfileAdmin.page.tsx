import { render, screen, type RenderResult } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { vi } from "vitest";
import { Main } from "../../app/App.tsx";

vi.mock("../Home", () => {
    return {
        Home: () => {
            return (
                <div data-testid="home-page">I am the mocked Home component</div>
            );
        }
    }
});

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

export function getTicketsList(): string[] {
    const elements = renderedComponent.container.querySelectorAll('[data-testid="ticket-item"]');
    return Array.from(elements).map(element => element.textContent || '');
}

export function getStatsCards(): string[] {
    const statCards = renderedComponent.container.querySelectorAll('[data-testid="stat-card"]');
    return Array.from(statCards).map(card => card.textContent || '');
}
