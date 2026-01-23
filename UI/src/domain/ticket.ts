import { z } from "zod";

export const TicketSchema = z.object({
    Id: z.string(),
    EventId: z.string(),
    Price: z.number().min(0),
    SeatNumber: z.number().int().min(1),
    Purchased: z.boolean()
});

export type Ticket = z.infer<typeof TicketSchema>;
