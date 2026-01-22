import { describe, it } from "vitest";
import {
    should_fetch_notifications_on_mount,
    should_return_empty_array_when_no_notifications,
    should_not_fetch_when_not_authenticated,
    should_return_loading_state_initially,
    should_pass_authorization_header,
    should_provide_refetch_function,
    should_identify_unread_notifications
} from "./useNotifications.steps";

describe("useNotifications", () => {
    it("should fetch notifications on mount", should_fetch_notifications_on_mount);
    it("should return empty array when no notifications", should_return_empty_array_when_no_notifications);
    it("should not fetch when not authenticated", should_not_fetch_when_not_authenticated);
    it("should return loading state initially", should_return_loading_state_initially);
    it("should pass authorization header", should_pass_authorization_header);
    it("should provide refetch function", should_provide_refetch_function);
    it("should identify unread notifications", should_identify_unread_notifications);
});
