import {describe, it} from "vitest";
import {
    should_display_user_profile_information,
    should_display_user_tickets,
    should_navigate_back_to_home,
    should_show_loading_while_fetching_tickets
} from "./UserProfile.steps.ts";

describe("UserProfile", () => {
    it("should display user profile information", should_display_user_profile_information);
    it("should display user tickets", should_display_user_tickets);
    it("should navigate back to home when back button is clicked", should_navigate_back_to_home);
    it("should show loading while fetching tickets", should_show_loading_while_fetching_tickets);
});
