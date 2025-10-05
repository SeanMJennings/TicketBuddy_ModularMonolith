import {describe, it} from "vitest";
import {
    should_display_seat_map_for_event,
    should_navigate_back_to_events,
    should_allow_selecting_multiple_seats_and_proceed_to_purchase,
    should_not_allow_selecting_a_purchased_ticket,
        should_show_error_if_fail_to_reserve_ticket,
    should_organize_selected_seats_in_ascending_order
} from "./Tickets.steps.ts";

describe("Tickets", () => {
    it("should display seat map for event", should_display_seat_map_for_event);
    it("should navigate back to events page when Back to Events button is clicked", should_navigate_back_to_events);
    it("should allow selecting multiple seats and proceed to purchase", should_allow_selecting_multiple_seats_and_proceed_to_purchase);
    it('should show error if fail to reserve ticket', should_show_error_if_fail_to_reserve_ticket);
    it("should not allow selecting a purchased ticket", should_not_allow_selecting_a_purchased_ticket);
    it("should organize selected seats in ascending order", should_organize_selected_seats_in_ascending_order);
});
