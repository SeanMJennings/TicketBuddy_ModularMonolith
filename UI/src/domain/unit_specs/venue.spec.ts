import { describe, it, expect } from "vitest";
import { VenueSchema, VenuePayloadSchema } from "../venue";

describe("Venue Schema", () => {
    it("should parse a valid venue object", () => {
        const validVenue = {
            Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            Name: "The Grand Theater",
            Address: {
                Street: "123 Main Street",
                City: "London",
                Postcode: "SW1A 1AA"
            },
            Capacity: 25
        };

        const result = VenueSchema.safeParse(validVenue);

        expect(result.success).toBe(true);
        if (result.success) {
            expect(result.data.Id).toBe(validVenue.Id);
            expect(result.data.Name).toBe(validVenue.Name);
            expect(result.data.Address.Street).toBe(validVenue.Address.Street);
            expect(result.data.Address.City).toBe(validVenue.Address.City);
            expect(result.data.Address.Postcode).toBe(validVenue.Address.Postcode);
            expect(result.data.Capacity).toBe(validVenue.Capacity);
        }
    });

    it("should reject venue with missing required fields", () => {
        const invalidVenue = {
            Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            Name: "The Grand Theater"
        };

        const result = VenueSchema.safeParse(invalidVenue);

        expect(result.success).toBe(false);
    });

    it("should reject venue with negative capacity", () => {
        const invalidVenue = {
            Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            Name: "The Grand Theater",
            Address: {
                Street: "123 Main Street",
                City: "London",
                Postcode: "SW1A 1AA"
            },
            Capacity: -5
        };

        const result = VenueSchema.safeParse(invalidVenue);

        expect(result.success).toBe(false);
    });

    it("should reject venue with capacity above 50", () => {
        const invalidVenue = {
            Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            Name: "The Grand Theater",
            Address: {
                Street: "123 Main Street",
                City: "London",
                Postcode: "SW1A 1AA"
            },
            Capacity: 51
        };

        const result = VenueSchema.safeParse(invalidVenue);

        expect(result.success).toBe(false);
    });

    it("should accept venue with capacity of exactly 50", () => {
        const validVenue = {
            Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            Name: "The Grand Theater",
            Address: {
                Street: "123 Main Street",
                City: "London",
                Postcode: "SW1A 1AA"
            },
            Capacity: 50
        };

        const result = VenueSchema.safeParse(validVenue);

        expect(result.success).toBe(true);
    });

    it("should accept venue with capacity of exactly 1", () => {
        const validVenue = {
            Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            Name: "Small Venue",
            Address: {
                Street: "123 Main Street",
                City: "London",
                Postcode: "SW1A 1AA"
            },
            Capacity: 1
        };

        const result = VenueSchema.safeParse(validVenue);

        expect(result.success).toBe(true);
    });
});

describe("VenuePayload Schema", () => {
    it("should parse a valid venue payload without id", () => {
        const validPayload = {
            Name: "The Grand Theater",
            Street: "123 Main Street",
            City: "London",
            Postcode: "SW1A 1AA",
            Capacity: 25
        };

        const result = VenuePayloadSchema.safeParse(validPayload);

        expect(result.success).toBe(true);
        if (result.success) {
            expect(result.data.Name).toBe(validPayload.Name);
            expect(result.data.Street).toBe(validPayload.Street);
            expect(result.data.Capacity).toBe(validPayload.Capacity);
        }
    });

    it("should reject payload that includes an id field", () => {
        const payloadWithId = {
            Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            Name: "The Grand Theater",
            Street: "123 Main Street",
            City: "London",
            Postcode: "SW1A 1AA",
            Capacity: 25
        };

        const result = VenuePayloadSchema.strict().safeParse(payloadWithId);

        expect(result.success).toBe(false);
    });
});
