import { z } from "zod";

const AddressSchema = z.object({
    Street: z.string(),
    City: z.string(),
    Postcode: z.string()
});

export const VenuePayloadSchema = z.object({
    Name: z.string(),
    Street: z.string(),
    City: z.string(),
    Postcode: z.string(),
    Capacity: z.number().int().min(1).max(50)
});

export const VenueSchema = z.object({
    Id: z.string(),
    Name: z.string(),
    Address: AddressSchema,
    Capacity: z.number().int().min(1).max(50)
});

export type Address = z.infer<typeof AddressSchema>;
export type VenuePayload = z.infer<typeof VenuePayloadSchema>;
export type Venue = z.infer<typeof VenueSchema>;
