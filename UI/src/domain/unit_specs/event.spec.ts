import { describe, it } from "vitest";
import {
    should_parse_valid_event,
    should_parse_sold_out_event,
    should_reject_event_with_missing_fields,
    should_reject_event_with_negative_price,
    should_accept_event_with_price_zero,
    should_reject_event_with_invalid_start_date,
    should_reject_event_with_invalid_end_date,
    should_accept_event_with_zulu_timezone,
    should_parse_valid_event_payload,
    should_reject_event_payload_with_id,
    should_parse_valid_update_event_payload,
    should_reject_update_payload_with_venue_id
} from "./event.steps";

describe("Event Schema", () => {
    it("should parse a valid event object", should_parse_valid_event);
    it("should parse a sold out event", should_parse_sold_out_event);
    it("should reject event with missing required fields", should_reject_event_with_missing_fields);
    it("should reject event with negative price", should_reject_event_with_negative_price);
    it("should accept event with price of zero", should_accept_event_with_price_zero);
    it("should reject event with invalid start date", should_reject_event_with_invalid_start_date);
    it("should reject event with invalid end date", should_reject_event_with_invalid_end_date);
    it("should accept event with zulu timezone format", should_accept_event_with_zulu_timezone);
});

describe("EventPayload Schema", () => {
    it("should parse a valid event payload", should_parse_valid_event_payload);
    it("should reject event payload that includes an id field", should_reject_event_payload_with_id);
});

describe("UpdateEventPayload Schema", () => {
    it("should parse a valid update event payload", should_parse_valid_update_event_payload);
    it("should reject update payload that includes venue id", should_reject_update_payload_with_venue_id);
});
