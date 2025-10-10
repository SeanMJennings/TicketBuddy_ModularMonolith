import {vi} from 'vitest';
import {MockServer} from "../../testing/mock-server.ts";
import {afterEach, beforeEach} from "vitest";
import {Events, Users} from "../../testing/data.ts";
import {
    clickFindTicketsButton,
    eventExists,
    renderHome,
    soldOutMessageExists,
    unmountHome
} from "./Home.page.tsx";
import {waitUntil} from "../../testing/utilities.ts";
import {expect} from "vitest";

const mockedUseNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
    const mod = await vi.importActual<typeof import("react-router-dom")>(
        "react-router-dom"
    );
    return {
        ...mod,
        useNavigate: () => mockedUseNavigate,
    };
});

const mockServer = MockServer.New();
let wait_for_get_events: () => boolean;
let wait_for_get_users: () => boolean;

beforeEach(() => {
    mockServer.reset();
    wait_for_get_events = mockServer.get("events", Events)
    wait_for_get_users = mockServer.get("users", Users);
    mockServer.start();
});

afterEach(() => {
    unmountHome();
});

export async function should_load_events_on_render() {
    renderHome();
    await waitUntil(wait_for_get_events);
    await waitUntil(wait_for_get_users);
    for (const event of Events) {
        expect(eventExists(event.EventName)).toBeTruthy();
    }
}

export async function should_navigate_to_tickets_page_when_find_tickets_clicked() {
    renderHome();
    await waitUntil(wait_for_get_events);
    await waitUntil(wait_for_get_users);
    await clickFindTicketsButton(0);
    expect(mockedUseNavigate).toHaveBeenCalledWith("/tickets/1");
}

export async function should_show_sold_out_message_for_sold_out_events() {
    renderHome();
    await waitUntil(wait_for_get_events);
    await waitUntil(wait_for_get_users);

    expect(soldOutMessageExists("Football Match at Wembley Stadium")).toBeTruthy();
}

