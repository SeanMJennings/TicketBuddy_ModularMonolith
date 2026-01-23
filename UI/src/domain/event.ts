import { z } from "zod";

export const EventSchema = z.object({
    Id: z.string(),
    EventName: z.string(),
    StartDate: z.string().datetime({ offset: true }),
    EndDate: z.string().datetime({ offset: true }),
    VenueId: z.string(),
    Price: z.number().min(0),
    IsSoldOut: z.boolean()
});

export const EventPayloadSchema = z.object({
    EventName: z.string(),
    StartDate: z.string().datetime({ offset: true }),
    EndDate: z.string().datetime({ offset: true }),
    VenueId: z.string(),
    Price: z.number().min(0)
});

export const UpdateEventPayloadSchema = z.object({
    EventName: z.string(),
    StartDate: z.string().datetime({ offset: true }),
    EndDate: z.string().datetime({ offset: true }),
    Price: z.number().min(0)
});

export type Event = z.infer<typeof EventSchema>;
export type EventPayload = z.infer<typeof EventPayloadSchema>;
export type UpdateEventPayload = z.infer<typeof UpdateEventPayloadSchema>;