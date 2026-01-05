import { get, post, put } from "../common/http";
import { VenueSchema, type Venue, type VenuePayload } from "../domain/venue";
import { z } from "zod";

const VenuesArraySchema = z.array(VenueSchema);

export const getVenues = async (): Promise<Venue[]> => {
    const response = await get<unknown[]>("/venues");
    return VenuesArraySchema.parse(response);
};

export const getVenueById = async (id: string): Promise<Venue> => {
    const response = await get<unknown>(`/venues/${id}`);
    return VenueSchema.parse(response);
};

export const createVenue = async (venue: VenuePayload, jwt: string): Promise<void> => {
    await post("/venues", venue, jwt);
};

export const updateVenue = async (id: string, venue: VenuePayload, jwt: string): Promise<void> => {
    await put(`/venues/${id}`, venue, jwt);
};
