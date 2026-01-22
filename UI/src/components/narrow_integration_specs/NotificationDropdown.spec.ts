import { describe, it } from "vitest";
import {
    should_render_list_of_notifications,
    should_show_event_name_for_ticket_purchased_notification,
    should_show_empty_state_when_no_notifications,
    should_show_loading_state_while_fetching,
    should_highlight_unread_notifications,
    should_call_mark_as_read_when_notification_clicked,
    should_refetch_notifications_after_marking_as_read
} from "./NotificationDropdown.steps";

describe("NotificationDropdown", () => {
    it("should render list of notifications", should_render_list_of_notifications);
    it("should show event name for TicketPurchased notification", should_show_event_name_for_ticket_purchased_notification);
    it("should show empty state when no notifications", should_show_empty_state_when_no_notifications);
    it("should show loading state while fetching", should_show_loading_state_while_fetching);
    it("should highlight unread notifications", should_highlight_unread_notifications);
    it("should call mark as read when notification clicked", should_call_mark_as_read_when_notification_clicked);
    it("should refetch notifications after marking as read", should_refetch_notifications_after_marking_as_read);
});
