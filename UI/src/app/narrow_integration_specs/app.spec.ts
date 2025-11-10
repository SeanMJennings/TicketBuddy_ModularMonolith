import {describe, it} from "vitest";
import {
    should_default_to_home_page,
    should_display_event_management_navigation_if_user_is_admin,
    should_display_not_found_for_unknown_routes,
    should_navigate_to_events_management_page_when_link_is_clicked,
    should_navigate_to_home_page_when_ticket_logo_is_clicked,
    should_show_user_details_when_user_icon_is_clicked,
    should_redirect_to_error_page_on_server_error,
    should_let_a_user_login, should_let_a_user_logout,
    should_show_user_icon_when_user_logged_in
} from "./app.steps.ts";

describe('App', () => {
    it('should default to the home page', should_default_to_home_page);
    it('should display not found for unknown routes', should_display_not_found_for_unknown_routes);
    it('should let a user login', should_let_a_user_login);
    it('should let a user logout', should_let_a_user_logout);
    it('should load a user icon when a user is logged in', should_show_user_icon_when_user_logged_in);
    it('should show user details when user icon is clicked', should_show_user_details_when_user_icon_is_clicked);
    it('should display event management navigation if user is an admin', should_display_event_management_navigation_if_user_is_admin);
    it('should navigate to events management page when link is clicked', should_navigate_to_events_management_page_when_link_is_clicked);
    it('should navigate to home page when ticket logo is clicked', should_navigate_to_home_page_when_ticket_logo_is_clicked);
    it('should redirect to error page on server error', should_redirect_to_error_page_on_server_error);
});