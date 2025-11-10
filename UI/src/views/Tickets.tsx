import {useEffect, useState} from 'react';
import {useParams, Link, useNavigate} from 'react-router-dom';
import {getEventById} from '../api/events.api';
import {type Ticket} from '../domain/ticket';
import {type Event} from '../domain/event';
import {
    SeatMapContainer,
    SeatRow,
    Seat,
    ScreenArea,
    PriceInfo,
    Legend,
    LegendItem,
    LegendColor,
    SelectionInfo, CenteredButtonContainer, ActionBar
} from './Tickets.styles';
import {Button} from '../components/Button.styles';
import {BackIcon} from './EventsManagement.styles';
import {getTicketsForEvent, reserveTickets} from "../api/tickets.api.ts";
import {handleError} from "../common/tickets/ticket-errors.ts";
import {Container, PageTitle} from "./Common.styles.tsx";
import {ContentLoading} from "../components/LoadingContainers.styles.tsx";
import {convertToTicketBuddyUser} from "../oidc/key-cloak-user.extensions.ts";
import { useAuth } from 'react-oidc-context';

const SEATS_PER_ROW = 5;

export const Tickets = () => {
    const {eventId} = useParams<{ eventId: string }>();
    const [tickets, setTickets] = useState<Ticket[]>([]);
    const [event, setEvent] = useState<Event | null>(null);
    const [loading, setLoading] = useState(true);
    const [selectedSeats, setSelectedSeats] = useState<number[]>([]);
    const navigate = useNavigate();
    const [proceeding, setProceeding] = useState(false);

    const auth = useAuth();
    const user = convertToTicketBuddyUser(auth.user);

    useEffect(() => {
        const fetchEventAndTickets = async () => {
            if (!eventId) return;
            await Promise.all([
                getEventById(eventId),
                getTicketsForEvent(eventId)
            ]).then(data => {
                setEvent(data[0]);
                setTickets(data[1]);
                setLoading(false);
            }).catch(() => {
                setLoading(false);
            });
        };

        fetchEventAndTickets();
    }, [eventId]);

    const handleSeatClick = (seatNumber: number) => {
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
    };

    const proceedToPurchase = () => {
        if (selectedSeats.length > 0 && eventId) {
            setProceeding(true);
            reserveTickets(eventId, {
                UserId: user ? user.Id : '',
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
            })
        }
    };

    const renderSeatMap = () => {
        const maxSeatNumber = Math.max(...tickets.map(ticket => ticket.SeatNumber), 0);
        const numRows = Math.ceil(maxSeatNumber / SEATS_PER_ROW);

        const rows = [];
        for (let rowIndex = 0; rowIndex < numRows; rowIndex++) {
            const startSeat = rowIndex * SEATS_PER_ROW + 1;
            const endSeat = Math.min((rowIndex + 1) * SEATS_PER_ROW, maxSeatNumber);

            const seats = [];
            for (let seatNumber = startSeat; seatNumber <= endSeat; seatNumber++) {
                const ticket = tickets.find(t => t.SeatNumber === seatNumber);
                const isBooked = ticket ? ticket.Purchased : false;
                const isSelected = selectedSeats.includes(seatNumber);

                seats.push(
                    <Seat
                        key={seatNumber}
                        isbooked={isBooked.toString()}
                        isselected={isSelected.toString()}
                        className={isBooked ? 'booked' : isSelected ? 'selected' : ''}
                        data-seat={seatNumber}
                        onClick={() => handleSeatClick(seatNumber)}
                    >
                        {seatNumber}
                    </Seat>
                );
            }

            rows.push(
                <SeatRow key={rowIndex} data-row={rowIndex + 1}>
                    {seats}
                </SeatRow>
            );
        }

        return rows;
    };

    const calculateTotalPrice = () => {
        if (tickets.length === 0 || selectedSeats.length === 0) return 0;
        const ticketPrice = tickets[0].Price;
        return ticketPrice * selectedSeats.length;
    };

    return (
        <Container>
            <PageTitle>Tickets for Event: {event?.EventName}</PageTitle>
            <ActionBar>
                <Link to="/">
                    <Button data-testid="back-button">
                        <BackIcon/> Back to Events
                    </Button>
                </Link>
            </ActionBar>

            {loading ? (
                <ContentLoading />
            ) : (
                <>
                    <SeatMapContainer>
                        <ScreenArea>Screen / Stage</ScreenArea>
                        {renderSeatMap()}

                        <Legend>
                            <LegendItem>
                                <LegendColor color="#4CAF50"/>
                                <span>Available</span>
                            </LegendItem>
                            <LegendItem>
                                <LegendColor color="#f5f5f5"/>
                                <span>Booked</span>
                            </LegendItem>
                            <LegendItem>
                                <LegendColor color="#FF9800"/>
                                <span>Selected</span>
                            </LegendItem>
                        </Legend>

                        {selectedSeats.length > 0 && (
                            <SelectionInfo data-testid="selection-info">
                                <h3>Selected Seats</h3>
                                <p>Seats: {selectedSeats.join(', ')}</p>
                                <PriceInfo data-testid="price-info">
                                    Total: £{calculateTotalPrice().toFixed(2)}
                                </PriceInfo>
                                <CenteredButtonContainer>
                                    <Button
                                        onClick={proceedToPurchase}
                                        disabled={proceeding}
                                        data-testid="proceed-to-purchase"
                                    >
                                        {proceeding ? 'Processing...' : 'Proceed to Purchase'}
                                    </Button>
                                </CenteredButtonContainer>
                            </SelectionInfo>
                        )}
                    </SeatMapContainer>
                </>
            )}
        </Container>
    );
};
