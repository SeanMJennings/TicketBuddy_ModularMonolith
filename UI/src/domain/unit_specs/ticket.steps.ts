import { expect } from "vitest";
import { TicketSchema } from "../ticket";

export function should_parse_valid_ticket() {
    const validTicket = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 25.50,
        SeatNumber: 12,
        Purchased: false
    };

    const result = TicketSchema.safeParse(validTicket);

    expect(result.success).toBe(true);
    expect(result.data?.Id).toBe(validTicket.Id);
    expect(result.data?.EventId).toBe(validTicket.EventId);
    expect(result.data?.Price).toBe(validTicket.Price);
    expect(result.data?.SeatNumber).toBe(validTicket.SeatNumber);
    expect(result.data?.Purchased).toBe(validTicket.Purchased);
}

export function should_reject_ticket_with_missing_fields() {
    const invalidTicket = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
    };

    const result = TicketSchema.safeParse(invalidTicket);

    expect(result.success).toBe(false);
}

export function should_reject_ticket_with_negative_price() {
    const invalidTicket = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: -10,
        SeatNumber: 12,
        Purchased: false
    };

    const result = TicketSchema.safeParse(invalidTicket);

    expect(result.success).toBe(false);
}

export function should_reject_ticket_with_negative_seat_number() {
    const invalidTicket = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 25.50,
        SeatNumber: -1,
        Purchased: false
    };

    const result = TicketSchema.safeParse(invalidTicket);

    expect(result.success).toBe(false);
}

export function should_accept_ticket_with_price_zero() {
    const validTicket = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 0,
        SeatNumber: 12,
        Purchased: true
    };

    const result = TicketSchema.safeParse(validTicket);

    expect(result.success).toBe(true);
}

export function should_reject_ticket_with_non_boolean_purchased() {
    const invalidTicket = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 25.50,
        SeatNumber: 12,
        Purchased: "true"
    };

    const result = TicketSchema.safeParse(invalidTicket);

    expect(result.success).toBe(false);
}
