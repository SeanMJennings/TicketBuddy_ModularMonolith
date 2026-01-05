import { describe, it } from "vitest";
import {
    should_display_list_of_venues,
    should_allow_user_to_create_new_venue,
    should_navigate_back_to_venues_list_when_back_button_is_clicked,
    should_allow_user_to_edit_existing_venue
} from "./VenuesManagement.steps.ts";

describe("Venues Management", () => {
    it("should display list of venues", should_display_list_of_venues);
    it("should allow user to create a new venue", should_allow_user_to_create_new_venue);
    it("should navigate back to venues list when back button is clicked", should_navigate_back_to_venues_list_when_back_button_is_clicked);
    it("should allow user to edit an existing venue", should_allow_user_to_edit_existing_venue);
});
