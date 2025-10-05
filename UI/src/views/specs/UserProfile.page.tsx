import {render, type RenderResult} from "@testing-library/react";
import {MemoryRouter} from "react-router-dom";
import {vi} from "vitest";
import {userEvent} from "@testing-library/user-event";
import {Main} from "../../App.tsx";

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
            <Main/>
        </MemoryRouter>);
    return renderedComponent;
}

export function unmountUserProfile() {
    renderedComponent?.unmount();
}

export function getUserNameDisplay(): string | null {
    const element = elements.userNameDisplay();
    return element?.textContent || null;
}

export function getUserEmailDisplay(): string | null {
    const element = elements.userEmailDisplay();
    return element?.textContent || null;
}

export function getTicketsList(): string[] {
    const elements = renderedComponent.container.querySelectorAll('[data-testid="ticket-item"]');
    return Array.from(elements).map(element => element.textContent || '');
}

export function clickBackToHomeButton() {
    return userEvent.click(elements.backToHomeButton()!);
}

export function homePageIsRendered() {
    return elements.homePageIsRendered() !== null;
}

export function loadingIsDisplayed() {
    return elements.loadingIndicator() !== null;
}

export const elements = {
    userNameDisplay: () => renderedComponent.queryByTestId('user-name'),
    userEmailDisplay: () => renderedComponent.queryByTestId('user-email'),
    backToHomeButton: () => renderedComponent.getByText('Back to Home'),
    homePageIsRendered: () => renderedComponent.queryByTestId('home-page'),
    loadingIndicator: () => renderedComponent.queryByTestId('loading-indicator'),
};
