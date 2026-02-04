import { expect } from "vitest";
import { EventSchema, EventPayloadSchema, UpdateEventPayloadSchema } from "../event";

export function should_parse_valid_event() {
    const validEvent = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventName: "Jazz Evening",
        StartDate: "2026-02-15T19:00:00+00:00",
        EndDate: "2026-02-15T22:00:00+00:00",
        VenueId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 45.00,
        IsSoldOut: false
    };

    const result = EventSchema.safeParse(validEvent)

    expect(result.success).toBe(true);
    expect(result.data?.Id).toBe(validEvent.Id);
    expect(result.data?.EventName).toBe(validEvent.EventName);
    expect(result.data?.StartDate).toBe(validEvent.StartDate);
    expect(result.data?.EndDate).toBe(validEvent.EndDate);
    expect(result.data?.VenueId).toBe(validEvent.VenueId);
    expect(result.data?.Price).toBe(validEvent.Price);
    expect(result.data?.IsSoldOut).toBe(validEvent.IsSoldOut);
}

export function should_parse_sold_out_event() {
    const soldOutEvent = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventName: "Popular Concert",
        StartDate: "2026-02-15T19:00:00+00:00",
        EndDate: "2026-02-15T22:00:00+00:00",
        VenueId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 75.00,
        IsSoldOut: true
    };

    const result = EventSchema.safeParse(soldOutEvent);

    expect(result.success).toBe(true);
    expect(result.data?.IsSoldOut).toBe(true);
}

export function should_reject_event_with_missing_fields() {
    const invalidEvent = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventName: "Jazz Evening"
    };

    const result = EventSchema.safeParse(invalidEvent);

    expect(result.success).toBe(false);
}

export function should_reject_event_with_negative_price() {
    const invalidEvent = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventName: "Jazz Evening",
        StartDate: "2026-02-15T19:00:00+00:00",
        EndDate: "2026-02-15T22:00:00+00:00",
        VenueId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: -10,
        IsSoldOut: false
    };

    const result = EventSchema.safeParse(invalidEvent);

    expect(result.success).toBe(false);
}

export function should_accept_event_with_price_zero() {
    const freeEvent = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventName: "Free Community Event",
        StartDate: "2026-02-15T19:00:00+00:00",
        EndDate: "2026-02-15T22:00:00+00:00",
        VenueId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 0,
        IsSoldOut: false
    };

    const result = EventSchema.safeParse(freeEvent);

    expect(result.success).toBe(true);
}

export function should_parse_valid_event_payload() {
    const validPayload = {
        EventName: "Jazz Evening",
        StartDate: "2026-02-15T19:00:00+00:00",
        EndDate: "2026-02-15T22:00:00+00:00",
        VenueId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 45.00
    };

    const result = EventPayloadSchema.safeParse(validPayload);

    expect(result.success).toBe(true);
    expect(result.data?.EventName).toBe(validPayload.EventName);
    expect(result.data?.StartDate).toBe(validPayload.StartDate);
    expect(result.data?.EndDate).toBe(validPayload.EndDate);
    expect(result.data?.VenueId).toBe(validPayload.VenueId);
    expect(result.data?.Price).toBe(validPayload.Price);
}

export function should_reject_event_payload_with_id() {
    const payloadWithId = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventName: "Jazz Evening",
        StartDate: "2026-02-15T19:00:00+00:00",
        EndDate: "2026-02-15T22:00:00+00:00",
        VenueId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 45.00
    };

    const result = EventPayloadSchema.strict().safeParse(payloadWithId);

    expect(result.success).toBe(false);
}

export function should_parse_valid_update_event_payload() {
    const validPayload = {
        EventName: "Updated Jazz Evening",
        StartDate: "2026-02-15T19:00:00+00:00",
        EndDate: "2026-02-15T22:00:00+00:00",
        Price: 50.00
    };

    const result = UpdateEventPayloadSchema.safeParse(validPayload);

    expect(result.success).toBe(true);
    expect(result.data?.EventName).toBe(validPayload.EventName);
    expect(result.data?.StartDate).toBe(validPayload.StartDate);
    expect(result.data?.EndDate).toBe(validPayload.EndDate);
    expect(result.data?.Price).toBe(validPayload.Price);
}

export function should_reject_update_payload_with_venue_id() {
    const payloadWithVenueId = {
        EventName: "Updated Jazz Evening",
        StartDate: "2026-02-15T19:00:00+00:00",
        EndDate: "2026-02-15T22:00:00+00:00",
        VenueId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 50.00
    };

    const result = UpdateEventPayloadSchema.strict().safeParse(payloadWithVenueId);

    expect(result.success).toBe(false);
}

export function should_reject_event_with_invalid_start_date() {
    const invalidEvent = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventName: "Jazz Evening",
        StartDate: "not-a-valid-date",
        EndDate: "2026-02-15T22:00:00+00:00",
        VenueId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 45.00,
        IsSoldOut: false
    };

    const result = EventSchema.safeParse(invalidEvent);

    expect(result.success).toBe(false);
}

export function should_reject_event_with_invalid_end_date() {
    const invalidEvent = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventName: "Jazz Evening",
        StartDate: "2026-02-15T19:00:00+00:00",
        EndDate: "15/02/2026",
        VenueId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 45.00,
        IsSoldOut: false
    };

    const result = EventSchema.safeParse(invalidEvent);

    expect(result.success).toBe(false);
}

export function should_accept_event_with_zulu_timezone() {
    const eventWithZulu = {
        Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
        EventName: "Jazz Evening",
        StartDate: "2026-02-15T19:00:00Z",
        EndDate: "2026-02-15T22:00:00Z",
        VenueId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        Price: 45.00,
        IsSoldOut: false
    };

    const result = EventSchema.safeParse(eventWithZulu);

    expect(result.success).toBe(true);
}
