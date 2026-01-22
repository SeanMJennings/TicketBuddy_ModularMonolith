import { describe, it } from "vitest";
import {
    should_fetch_unread_count_on_mount,
    should_return_zero_when_no_unread_notifications,
    should_not_fetch_when_not_authenticated,
    should_return_loading_state_initially,
    should_pass_authorization_header,
    should_provide_refetch_function
} from "./useNotificationCount.steps";

describe("useNotificationCount", () => {
    it("should fetch unread count on mount", should_fetch_unread_count_on_mount);
    it("should return zero when no unread notifications", should_return_zero_when_no_unread_notifications);
    it("should not fetch when not authenticated", should_not_fetch_when_not_authenticated);
    it("should return loading state initially", should_return_loading_state_initially);
    it("should pass authorization header", should_pass_authorization_header);
    it("should provide refetch function", should_provide_refetch_function);
});
