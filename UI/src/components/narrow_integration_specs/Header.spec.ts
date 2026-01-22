import { describe, it } from "vitest";
import {
    should_show_notification_bell_when_authenticated,
    should_not_show_notification_bell_when_not_authenticated,
    should_show_badge_with_unread_count_when_count_greater_than_zero,
    should_not_show_badge_when_count_is_zero,
    should_not_show_badge_when_count_is_null
} from "./Header.steps";

describe("Header - Notifications", () => {
    it("should show notification bell when authenticated", should_show_notification_bell_when_authenticated);
    it("should not show notification bell when not authenticated", should_not_show_notification_bell_when_not_authenticated);
    it("should show badge with unread count when count > 0", should_show_badge_with_unread_count_when_count_greater_than_zero);
    it("should not show badge when count is zero", should_not_show_badge_when_count_is_zero);
    it("should not show badge when count is null", should_not_show_badge_when_count_is_null);
});
