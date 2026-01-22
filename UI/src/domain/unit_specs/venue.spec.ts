import { describe, it } from "vitest";
import {
    should_parse_valid_venue,
    should_reject_venue_with_missing_fields,
    should_reject_venue_with_negative_capacity,
    should_reject_venue_with_capacity_above_50,
    should_accept_venue_with_capacity_50,
    should_accept_venue_with_capacity_1,
    should_parse_valid_venue_payload,
    should_reject_payload_with_id
} from "./venue.steps";

describe("Venue Schema", () => {
    it("should parse a valid venue object", should_parse_valid_venue);
    it("should reject venue with missing required fields", should_reject_venue_with_missing_fields);
    it("should reject venue with negative capacity", should_reject_venue_with_negative_capacity);
    it("should reject venue with capacity above 50", should_reject_venue_with_capacity_above_50);
    it("should accept venue with capacity of exactly 50", should_accept_venue_with_capacity_50);
    it("should accept venue with capacity of exactly 1", should_accept_venue_with_capacity_1);
});

describe("VenuePayload Schema", () => {
    it("should parse a valid venue payload without id", should_parse_valid_venue_payload);
    it("should reject payload that includes an id field", should_reject_payload_with_id);
});
