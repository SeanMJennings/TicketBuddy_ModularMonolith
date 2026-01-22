import { describe, it } from "vitest";
import {
    should_fetch_all_notifications,
    should_return_validated_notification_objects,
    should_pass_authorization_header_when_fetching_notifications,
    should_return_empty_array_when_no_notifications,
    should_mark_notification_as_read,
    should_fetch_unread_count,
    should_return_zero_unread_count,
    should_pass_authorization_header_when_fetching_unread_count
} from "./notifications.api.steps";

describe("Notifications API", () => {
    describe("getNotifications", () => {
        it("should fetch all notifications", should_fetch_all_notifications);
        it("should return validated notification objects", should_return_validated_notification_objects);
        it("should pass authorization header", should_pass_authorization_header_when_fetching_notifications);
        it("should return empty array when no notifications", should_return_empty_array_when_no_notifications);
    });

    describe("markNotificationAsRead", () => {
        it("should mark notification as read", should_mark_notification_as_read);
    });

    describe("getUnreadCount", () => {
        it("should fetch unread count", should_fetch_unread_count);
        it("should return zero unread count", should_return_zero_unread_count);
        it("should pass authorization header", should_pass_authorization_header_when_fetching_unread_count);
    });
});
