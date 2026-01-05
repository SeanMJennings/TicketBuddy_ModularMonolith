import { describe, it, expect } from "vitest";
import { VenueSchema, VenuePayloadSchema } from "../venue";

describe("Venue Schema", () => {
    it("should parse a valid venue object", () => {
        const validVenue = {
            id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            name: "The Grand Theater",
            address: {
                street: "123 Main Street",
                city: "London",
                postCode: "SW1A 1AA"
            },
            capacity: 25
        };

        const result = VenueSchema.safeParse(validVenue);

        expect(result.success).toBe(true);
        if (result.success) {
            expect(result.data.id).toBe(validVenue.id);
            expect(result.data.name).toBe(validVenue.name);
            expect(result.data.address.street).toBe(validVenue.address.street);
            expect(result.data.address.city).toBe(validVenue.address.city);
            expect(result.data.address.postCode).toBe(validVenue.address.postCode);
            expect(result.data.capacity).toBe(validVenue.capacity);
        }
    });

    it("should reject venue with missing required fields", () => {
        const invalidVenue = {
            id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            name: "The Grand Theater"
        };

        const result = VenueSchema.safeParse(invalidVenue);

        expect(result.success).toBe(false);
    });

    it("should reject venue with negative capacity", () => {
        const invalidVenue = {
            id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            name: "The Grand Theater",
            address: {
                street: "123 Main Street",
                city: "London",
                postCode: "SW1A 1AA"
            },
            capacity: -5
        };

        const result = VenueSchema.safeParse(invalidVenue);

        expect(result.success).toBe(false);
    });

    it("should reject venue with capacity above 50", () => {
        const invalidVenue = {
            id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            name: "The Grand Theater",
            address: {
                street: "123 Main Street",
                city: "London",
                postCode: "SW1A 1AA"
            },
            capacity: 51
        };

        const result = VenueSchema.safeParse(invalidVenue);

        expect(result.success).toBe(false);
    });

    it("should accept venue with capacity of exactly 50", () => {
        const validVenue = {
            id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            name: "The Grand Theater",
            address: {
                street: "123 Main Street",
                city: "London",
                postCode: "SW1A 1AA"
            },
            capacity: 50
        };

        const result = VenueSchema.safeParse(validVenue);

        expect(result.success).toBe(true);
    });

    it("should accept venue with capacity of exactly 1", () => {
        const validVenue = {
            id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            name: "Small Venue",
            address: {
                street: "123 Main Street",
                city: "London",
                postCode: "SW1A 1AA"
            },
            capacity: 1
        };

        const result = VenueSchema.safeParse(validVenue);

        expect(result.success).toBe(true);
    });
});

describe("VenuePayload Schema", () => {
    it("should parse a valid venue payload without id", () => {
        const validPayload = {
            name: "The Grand Theater",
            address: {
                street: "123 Main Street",
                city: "London",
                postCode: "SW1A 1AA"
            },
            capacity: 25
        };

        const result = VenuePayloadSchema.safeParse(validPayload);

        expect(result.success).toBe(true);
        if (result.success) {
            expect(result.data.name).toBe(validPayload.name);
            expect(result.data.address.street).toBe(validPayload.address.street);
            expect(result.data.capacity).toBe(validPayload.capacity);
        }
    });

    it("should reject payload that includes an id field", () => {
        const payloadWithId = {
            id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            name: "The Grand Theater",
            address: {
                street: "123 Main Street",
                city: "London",
                postCode: "SW1A 1AA"
            },
            capacity: 25
        };

        const result = VenuePayloadSchema.strict().safeParse(payloadWithId);

        expect(result.success).toBe(false);
    });
});
