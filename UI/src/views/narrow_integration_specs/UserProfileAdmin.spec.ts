import {describe, it} from "vitest";
import {
    should_not_display_bookings_section_for_admin_user
} from "./UserProfileAdmin.steps.ts";

describe("UserProfile for Admin", () => {
    it("should not display bookings section for admin user", should_not_display_bookings_section_for_admin_user);
});
