import { describe, it } from "vitest";
import {
    should_parse_valid_notification,
    should_parse_read_notification,
    should_reject_notification_with_missing_fields,
    should_reject_notification_with_invalid_isread_type,
    should_parse_valid_unread_count,
    should_parse_zero_unread_count,
    should_reject_non_integer_count,
    should_reject_negative_count,
    should_parse_ticket_purchased_payload,
    should_return_unknown_for_unrecognized_type,
    should_return_unknown_for_invalid_json,
    should_return_unknown_for_missing_required_payload_fields
} from "./notification.steps";

describe("Notification Schema", () => {
    it("should parse a valid notification object", should_parse_valid_notification);
    it("should parse a read notification", should_parse_read_notification);
    it("should reject notification with missing required fields", should_reject_notification_with_missing_fields);
    it("should reject notification with invalid IsRead type", should_reject_notification_with_invalid_isread_type);
});

describe("UnreadCount Schema", () => {
    it("should parse a valid unread count response", should_parse_valid_unread_count);
    it("should parse zero unread count", should_parse_zero_unread_count);
    it("should reject non-integer count", should_reject_non_integer_count);
    it("should reject negative count", should_reject_negative_count);
});

describe("parseNotificationPayload", () => {
    it("should parse TicketPurchased payload", should_parse_ticket_purchased_payload);
    it("should return unknown type for unrecognized notification type", should_return_unknown_for_unrecognized_type);
    it("should return unknown type for invalid JSON payload", should_return_unknown_for_invalid_json);
    it("should return unknown type for TicketPurchased with missing required fields", should_return_unknown_for_missing_required_payload_fields);
});
