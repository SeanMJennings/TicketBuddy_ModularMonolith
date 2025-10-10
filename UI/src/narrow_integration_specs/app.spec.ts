import {describe, it} from "vitest";
import {
    should_change_user_details_when_a_different_user_is_selected,
    should_default_to_home_page,
    should_display_event_management_navigation_if_user_is_admin,
    should_display_not_found_for_unknown_routes,
    should_load_list_of_users_to_select_from,
    should_navigate_to_events_management_page_when_link_is_clicked,
    should_navigate_to_home_page_when_ticket_logo_is_clicked,
    should_show_user_details_when_user_icon_is_clicked,
    should_show_user_icon_when_selected
} from "./app.steps.ts";

describe('App', () => {
    it('should default to the home page', should_default_to_home_page);
    it('should display not found for unknown routes', should_display_not_found_for_unknown_routes);
    it('should load list of users to select from', should_load_list_of_users_to_select_from);
    it('should load show a user icon when a user is selected', should_show_user_icon_when_selected);
    it('should show user details when user icon is clicked', should_show_user_details_when_user_icon_is_clicked);
    it('should change user details when a different user is selected', should_change_user_details_when_a_different_user_is_selected);
    it('should display event management navigation if user is an admin', should_display_event_management_navigation_if_user_is_admin);
    it('should navigate to events management page when link is clicked', should_navigate_to_events_management_page_when_link_is_clicked);
    it('should navigate to home page when ticket logo is clicked', should_navigate_to_home_page_when_ticket_logo_is_clicked);
});