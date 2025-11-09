import {vi} from 'vitest';
import {render, type RenderResult} from "@testing-library/react";
import App from "../App.tsx";
import {MemoryRouter} from "react-router-dom";
import {userEvent} from "@testing-library/user-event";

let renderedComponent: RenderResult;

vi.mock("../../views/Home", () => {
    return {
        Home: () => {
            return (
                <div>I am the mocked Home component</div>
            );
        }
    }
})

vi.mock("../../views/EventsManagement", () => {
    return {
        EventsManagement: () => {
            return (
                <div>I am the mocked Home events management</div>
            );
        }
    }
})

export function renderApp() {
    renderedComponent = render(
            <MemoryRouter>
                <App/>
            </MemoryRouter>)
    return renderedComponent;
}

export function renderAppAtEventsManagement() {
    renderedComponent = render(
            <MemoryRouter initialEntries={['/events-management']}>
                <App/>
            </MemoryRouter>)
    return renderedComponent;
}

export function renderAppAtUnknownRoute() {
    renderedComponent = render(
            <MemoryRouter initialEntries={['/wibble']}>
                <App/>
            </MemoryRouter>)
    return renderedComponent;
}

export function renderAppAtError() {
    renderedComponent = render(
            <MemoryRouter initialEntries={['/error']}>
                <App/>
            </MemoryRouter>)
    return renderedComponent;
}

export function unmountApp() {
    renderedComponent.unmount();
}

export function homePageIsRendered() {
    return elements.home() !== null;
}

export function errorPageIsRendered() {
    return elements.errorPage() !== null;
}

export function notFoundIsRendered() {
    return elements.notFound() !== null;
}

export function usersDropdownIsRendered() {
    return elements.usersDropdown() !== null;
}

export function userIconIsRendered() {
    return elements.userIcon() !== null;
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

export async function clickUserIcon() {
    const theUserIcon = await elements.theUserIcon();
    return userEvent.click(theUserIcon);
}

export function userEmailIsRendered(email: string) {
    return elements.userEmail(email);
}

export function eventsManagementLinkIsRendered() {
    return elements.eventsManagementLink() !== null;
}

export async function clickEventsManagementLink() {
    const link = await elements.theEventsManagementLink();
    return userEvent.click(link);
}

export function eventsManagementPageIsRendered() {
    return elements.eventsManagementPage() !== null;
}

export function ticketLogoIsRendered() {
    return elements.ticketLogo() !== null;
}

export async function clickTicketLogo() {
    const logo = await elements.theTicketLogo();
    return userEvent.click(logo);
}

export function userProfilePageIsRendered() {
    return elements.userProfilePage() !== null;
}

const elements = {
    home: () => renderedComponent.queryByText("I am the mocked Home component"),
    notFound: () => renderedComponent.queryByText("Page not found"),
    usersDropdown: () => renderedComponent.queryByTestId("users-dropdown"),
    theUsersDropdown: () => renderedComponent.findByTestId("users-dropdown"),
    userIcon: () => renderedComponent.queryByTestId("user-icon"),
    theUserIcon: () => renderedComponent.findByTestId("user-icon"),
    userEmail: (email: string) => renderedComponent.findByText(email),
    eventsManagementLink: () => renderedComponent.queryByText("Events Management"),
    theEventsManagementLink: () => renderedComponent.findByText("Events Management"),
    eventsManagementPage: () => renderedComponent.queryByText("I am the mocked Home events management"),
    ticketLogo: () => renderedComponent.queryByAltText("Ticket Stub"),
    theTicketLogo: () => renderedComponent.findByAltText("Ticket Stub"),
    userProfilePage: () => renderedComponent.queryByRole("heading", { name: "User Profile" }),
    errorPage: () => renderedComponent.queryByText("We're sorry, but an unexpected error occurred. Please try again later."),
}