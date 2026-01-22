import { describe, it } from "vitest";
import {
    should_render_list_of_notifications,
    should_show_event_name_for_ticket_purchased_notification,
    should_show_empty_state_when_no_notifications,
    should_show_loading_state_while_fetching,
    should_highlight_unread_notifications
} from "./NotificationDropdown.steps";

describe("NotificationDropdown", () => {
    it("should render list of notifications", should_render_list_of_notifications);
    it("should show event name for TicketPurchased notification", should_show_event_name_for_ticket_purchased_notification);
    it("should show empty state when no notifications", should_show_empty_state_when_no_notifications);
    it("should show loading state while fetching", should_show_loading_state_while_fetching);
    it("should highlight unread notifications", should_highlight_unread_notifications);
});
