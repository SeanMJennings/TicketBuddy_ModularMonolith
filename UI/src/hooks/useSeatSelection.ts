import { useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "react-oidc-context";
import { reserveTickets } from "../api/tickets.api";
import { handleError } from "../common/tickets/ticket-errors";
import { type Ticket } from "../domain/ticket";
import { type Event } from "../domain/event";

type UseSeatSelectionResult = {
    selectedSeats: number[];
    proceeding: boolean;
    handleSeatClick: (seatNumber: number) => void;
    proceedToPurchase: () => void;
};

type UseSeatSelectionOptions = {
    eventId: string | undefined;
    tickets: Ticket[];
    event: Event | null;
};

export const useSeatSelection = ({ eventId, tickets, event }: UseSeatSelectionOptions): UseSeatSelectionResult => {
    const [selectedSeats, setSelectedSeats] = useState<number[]>([]);
    const [proceeding, setProceeding] = useState(false);
    const navigate = useNavigate();
    const auth = useAuth();

    const handleSeatClick = useCallback((seatNumber: number) => {
        const ticket = tickets.find(t => t.SeatNumber === seatNumber);
        if (ticket?.Purchased) {
            return;
        }

        setSelectedSeats(prevSelectedSeats => {
            if (prevSelectedSeats.includes(seatNumber)) {
                return prevSelectedSeats.filter(seat => seat !== seatNumber);
            } else {
                return [...prevSelectedSeats, seatNumber].sort((a, b) => a - b);
            }
        });
    }, [tickets]);

    const proceedToPurchase = useCallback(() => {
        if (selectedSeats.length > 0 && eventId) {
            setProceeding(true);
            reserveTickets(eventId, {
                TicketIds: tickets.filter(t => selectedSeats.includes(t.SeatNumber)).map(t => t.Id)
            }, auth.user?.access_token
            ).then(() => {
                navigate(`/tickets/${eventId}/purchase`, {
                    state: {
                        selectedTickets: tickets.filter(t => selectedSeats.includes(t.SeatNumber)),
                        event: event
                    }
                });
            })
            .catch(handleError)
            .finally(() => {
                setProceeding(false);
            });
        }
    }, [selectedSeats, eventId, tickets, event, auth.user?.access_token, navigate]);

    return {
        selectedSeats,
        proceeding,
        handleSeatClick,
        proceedToPurchase
    };
};
