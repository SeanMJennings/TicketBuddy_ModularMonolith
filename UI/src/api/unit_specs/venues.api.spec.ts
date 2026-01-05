import { describe, it, beforeEach, expect } from "vitest";
import { MockServer } from "../../testing/mock-server";
import { waitUntil } from "../../testing/utilities";
import { getVenues, getVenueById, createVenue, updateVenue } from "../venues.api";
import { VenueSchema, type VenuePayload } from "../../domain/venue";

const mockServer = MockServer.New();

const validVenueResponse = {
    id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
    name: "The Grand Theater",
    address: {
        street: "123 Main Street",
        city: "London",
        postCode: "SW1A 1AA"
    },
    capacity: 25
};

const anotherVenueResponse = {
    id: "4f2504e0-4f89-11d3-9a0c-0305e82c3302",
    name: "The Royal Hall",
    address: {
        street: "456 High Street",
        city: "Manchester",
        postCode: "M1 1AA"
    },
    capacity: 40
};

beforeEach(() => {
    mockServer.reset();
    mockServer.start();
});

describe("Venues API", () => {
    describe("getVenues", () => {
        it("should fetch all venues", async () => {
            const wasCalled = mockServer.get("/venues", [validVenueResponse, anotherVenueResponse]);

            const venues = await getVenues();

            await waitUntil(wasCalled);
            expect(venues).toHaveLength(2);
            expect(venues[0].name).toBe("The Grand Theater");
            expect(venues[1].name).toBe("The Royal Hall");
        });

        it("should return validated venue objects", async () => {
            mockServer.get("/venues", [validVenueResponse]);

            const venues = await getVenues();

            const parseResult = VenueSchema.safeParse(venues[0]);
            expect(parseResult.success).toBe(true);
        });
    });

    describe("getVenueById", () => {
        it("should fetch a single venue by id", async () => {
            const venueId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
            mockServer.get(`/venues/${venueId}`, validVenueResponse);

            const venue = await getVenueById(venueId);

            expect(venue.id).toBe(venueId);
            expect(venue.name).toBe("The Grand Theater");
            expect(venue.address.city).toBe("London");
        });
    });

    describe("createVenue", () => {
        it("should create a new venue", async () => {
            const payload: VenuePayload = {
                name: "New Venue",
                address: {
                    street: "789 New Street",
                    city: "Birmingham",
                    postCode: "B1 1AA"
                },
                capacity: 50
            };
            const jwt = "admin-token";
            const wasCalled = mockServer.post("/venues", undefined, 201);

            await createVenue(payload, jwt);

            await waitUntil(wasCalled);
            expect(mockServer.headers.get("Authorization")).toBe(`Bearer ${jwt}`);
        });
    });

    describe("updateVenue", () => {
        it("should update an existing venue", async () => {
            const venueId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
            const payload: VenuePayload = {
                name: "Updated Venue",
                address: {
                    street: "123 Main Street",
                    city: "London",
                    postCode: "SW1A 1AA"
                },
                capacity: 30
            };
            const jwt = "admin-token";
            const wasCalled = mockServer.put(`/venues/${venueId}`, {});

            await updateVenue(venueId, payload, jwt);

            await waitUntil(wasCalled);
            expect(mockServer.headers.get("Authorization")).toBe(`Bearer ${jwt}`);
        });
    });
});
