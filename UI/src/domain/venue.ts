import { z } from "zod";

const AddressSchema = z.object({
    street: z.string(),
    city: z.string(),
    postCode: z.string()
});

export const VenuePayloadSchema = z.object({
    name: z.string(),
    address: AddressSchema,
    capacity: z.number().int().min(1).max(50)
});

export const VenueSchema = VenuePayloadSchema.extend({
    id: z.string()
});

export type Address = z.infer<typeof AddressSchema>;
export type VenuePayload = z.infer<typeof VenuePayloadSchema>;
export type Venue = z.infer<typeof VenueSchema>;
