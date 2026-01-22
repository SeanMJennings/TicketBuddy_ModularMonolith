import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router-dom";
import { NotificationDropdown } from "../NotificationDropdown";

let renderResult: ReturnType<typeof render>;

export function renderNotificationDropdown() {
    renderResult = render(
        <MemoryRouter>
            <NotificationDropdown />
        </MemoryRouter>
    );
}

export function unmountNotificationDropdown() {
    renderResult?.unmount();
}

export function getNotificationItems(): HTMLElement[] {
    return screen.queryAllByTestId("notification-item");
}

export function getNotificationItemCount(): number {
    return getNotificationItems().length;
}

export function getNotificationText(index: number): string | null {
    const items = getNotificationItems();
    return items[index]?.textContent ?? null;
}

export function emptyStateIsRendered(): boolean {
    return screen.queryByTestId("notifications-empty") !== null;
}

export function loadingStateIsRendered(): boolean {
    return screen.queryByTestId("notifications-loading") !== null;
}

export function isNotificationUnread(index: number): boolean {
    const items = getNotificationItems();
    return items[index]?.getAttribute("data-unread") === "true";
}

export async function clickNotification(index: number): Promise<void> {
    const items = getNotificationItems();
    if (items[index]) {
        await userEvent.click(items[index]);
    }
}
