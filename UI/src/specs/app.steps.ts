﻿import {
    clickEventsManagementLink,
    clickUserIcon,
    clickUsersDropdown,
    eventsManagementLinkIsRendered,
    eventsManagementPageIsRendered,
    homePageIsRendered,
    renderApp,
    selectUserFromDropdown,
    unmountApp,
    userEmailIsRendered,
    userIconIsRendered,
    usersDropdownIsRendered
} from "./app.page";
import {afterEach, beforeEach, expect} from "vitest";
import {MockServer} from "../testing/mock-server";
import {Users} from "../testing/data";
import {waitUntil} from "../testing/utilities";
import {userRoutes} from "../api/users.api";
import {UserType} from "../domain/user";

const mockServer = MockServer.New();
let wait_for_get: () => boolean;

beforeEach(() => {
    mockServer.reset();
    wait_for_get = mockServer.get(userRoutes.users, Users)
    mockServer.start();
});

afterEach(() => {
    unmountApp();
});

export function should_default_to_home_page() {
    renderApp();
    expect(homePageIsRendered()).toBeTruthy();
}

export async function should_load_list_of_users_to_select_from() {
    renderApp();
    await waitUntil(wait_for_get);
    expect(usersDropdownIsRendered()).toBeTruthy();
}

export async function should_show_user_icon_when_selected() {
    renderApp();
    await waitUntil(wait_for_get);
    expect(userIconIsRendered()).toBeTruthy();
}

export async function should_show_user_details_when_user_icon_is_clicked() {
    renderApp();
    await waitUntil(wait_for_get);
    await clickUserIcon();
    expect(await userEmailIsRendered(Users[0].Email)).toBeTruthy();
}

export async function should_change_user_details_when_a_different_user_is_selected() {
    renderApp();
    await waitUntil(wait_for_get);
    await clickUsersDropdown();
    await selectUserFromDropdown(Users[1].Id);
    await clickUserIcon();
    expect(await userEmailIsRendered(Users[1].Email)).toBeTruthy();
}

export async function should_display_event_management_navigation_if_user_is_admin() {
    renderApp();
    await waitUntil(wait_for_get);
    await clickUsersDropdown();
    await selectUserFromDropdown(Users.filter(u => u.UserType === UserType.Administrator)[0].Id);
    expect(eventsManagementLinkIsRendered()).toBeTruthy();
}

export async function should_navigate_to_events_management_page_when_link_is_clicked() {
    renderApp();
    await waitUntil(wait_for_get);
    await clickUsersDropdown();
    await selectUserFromDropdown(Users.filter(u => u.UserType === UserType.Administrator)[0].Id);
    expect(eventsManagementLinkIsRendered()).toBeTruthy();
    await clickEventsManagementLink();
    expect(eventsManagementPageIsRendered()).toBeTruthy();
}
