import { describe, it } from "vitest";
import {
    should_parse_valid_ticket,
    should_reject_ticket_with_missing_fields,
    should_reject_ticket_with_negative_price,
    should_reject_ticket_with_negative_seat_number,
    should_accept_ticket_with_price_zero,
    should_reject_ticket_with_non_boolean_purchased
} from "./ticket.steps";

describe("Ticket Schema", () => {
    it("should parse a valid ticket object", should_parse_valid_ticket);
    it("should reject ticket with missing required fields", should_reject_ticket_with_missing_fields);
    it("should reject ticket with negative price", should_reject_ticket_with_negative_price);
    it("should reject ticket with negative seat number", should_reject_ticket_with_negative_seat_number);
    it("should accept ticket with price of zero", should_accept_ticket_with_price_zero);
    it("should reject ticket with non-boolean purchased", should_reject_ticket_with_non_boolean_purchased);
});
