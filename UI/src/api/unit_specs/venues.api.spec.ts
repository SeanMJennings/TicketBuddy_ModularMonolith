import { describe, it, beforeEach, expect } from "vitest";
import { MockServer } from "../../testing/mock-server";
import { waitUntil } from "../../testing/utilities";
import { getVenues, getVenueById, createVenue, updateVenue } from "../venues.api";
import { VenueSchema, type VenuePayload } from "../../domain/venue";

const mockServer = MockServer.New();

const validVenueResponse = {
    Id: "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
    Name: "The Grand Theater",
    Address: {
        Street: "123 Main Street",
        City: "London",
        Postcode: "SW1A 1AA"
    },
    Capacity: 25
};

const anotherVenueResponse = {
    Id: "4f2504e0-4f89-11d3-9a0c-0305e82c3302",
    Name: "The Royal Hall",
    Address: {
        Street: "456 High Street",
        City: "Manchester",
        Postcode: "M1 1AA"
    },
    Capacity: 40
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
            expect(venues[0].Name).toBe("The Grand Theater");
            expect(venues[1].Name).toBe("The Royal Hall");
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

            expect(venue.Id).toBe(venueId);
            expect(venue.Name).toBe("The Grand Theater");
            expect(venue.Address.City).toBe("London");
        });
    });

    describe("createVenue", () => {
        it("should create a new venue", async () => {
            const payload: VenuePayload = {
                Name: "New Venue",
                Street: "789 New Street",
                City: "Birmingham",
                Postcode: "B1 1AA",
                Capacity: 50
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
                Name: "Updated Venue",
                Street: "123 Main Street",
                City: "London",
                Postcode: "SW1A 1AA",
                Capacity: 30
            };
            const jwt = "admin-token";
            const wasCalled = mockServer.put(`/venues/${venueId}`, {});

            await updateVenue(venueId, payload, jwt);

            await waitUntil(wasCalled);
            expect(mockServer.headers.get("Authorization")).toBe(`Bearer ${jwt}`);
        });
    });
});
