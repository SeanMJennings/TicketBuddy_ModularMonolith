import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router-dom";
import { Header } from "../Header";

let renderResult: ReturnType<typeof render>;

export function renderHeader() {
    renderResult = render(
        <MemoryRouter>
            <Header />
        </MemoryRouter>
    );
}

export function unmountHeader() {
    renderResult?.unmount();
}

export function notificationBellIsRendered(): boolean {
    return screen.queryByTestId("notification-bell") !== null;
}

export function notificationBadgeIsRendered(): boolean {
    return screen.queryByTestId("notification-badge") !== null;
}

export function getBadgeText(): string | null {
    return screen.queryByTestId("notification-badge")?.textContent ?? null;
}

export async function clickNotificationBell(): Promise<void> {
    const bell = screen.queryByTestId("notification-bell");
    if (bell) {
        await userEvent.click(bell);
    }
}

export function notificationDropdownIsRendered(): boolean {
    return screen.queryByTestId("notification-dropdown") !== null;
}
