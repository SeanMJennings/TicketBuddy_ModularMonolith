import {describe, it} from "vitest";
import {
    should_display_user_profile_information,
    should_display_user_tickets,
    should_navigate_back_to_home,
    should_show_loading_while_fetching_tickets,
    should_display_user_stats_when_tickets_exist,
    should_not_display_stats_when_no_tickets,
    should_display_event_names_in_tickets,
    should_display_event_date_and_venue_in_tickets,
    should_order_tickets_by_event_date_then_seat_number
} from "./UserProfile.steps.ts";

describe("UserProfile", () => {
    it("should display user profile information", should_display_user_profile_information);
    it("should display user tickets", should_display_user_tickets);
    it("should navigate back to home when back button is clicked", should_navigate_back_to_home);
    it("should show loading while fetching tickets", should_show_loading_while_fetching_tickets);
    it("should display user stats when tickets exist", should_display_user_stats_when_tickets_exist);
    it("should not display stats when no tickets", should_not_display_stats_when_no_tickets);
    it("should display event names in tickets", should_display_event_names_in_tickets);
    it("should display event date and venue in tickets", should_display_event_date_and_venue_in_tickets);
    it("should order tickets by event date then seat number", should_order_tickets_by_event_date_then_seat_number);
});
