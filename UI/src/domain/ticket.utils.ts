import { type Ticket } from "./ticket";
import { type Event } from "./event";
import { compareEventsByDate, findEventById } from "./event.utils";

export const calculateTotalSpent = (tickets: Ticket[]): number => {
    return tickets.reduce((total, ticket) => total + ticket.Price, 0);
};

export const calculateTotalPrice = (tickets: Ticket[], selectedSeats: number[]): number => {
    const ticketPrice = tickets[0].Price;
    return ticketPrice * selectedSeats.length;
};

export const sortTicketsByEventDateThenSeatNumber = (tickets: Ticket[], events: Event[]): Ticket[] => {
    const compareTickets = (ticketA: Ticket, ticketB: Ticket): number => {
        const eventA = findEventById(events, ticketA.EventId);
        const eventB = findEventById(events, ticketB.EventId);

        if (!eventA && !eventB) return 0;
        if (!eventA) return 1;
        if (!eventB) return -1;

        const dateDiff = compareEventsByDate(eventA, eventB);
        if (dateDiff !== 0) {
            return dateDiff;
        }

        return ticketA.SeatNumber - ticketB.SeatNumber;
    };

    return tickets.slice().sort(compareTickets);
};
