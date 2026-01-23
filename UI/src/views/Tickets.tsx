import {useParams, Link} from 'react-router-dom';
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
import {Container, PageTitle} from "./Common.styles.tsx";
import {ContentLoading} from "../components/LoadingContainers.styles.tsx";
import {calculateTotalPrice} from "../domain/ticket.utils";
import {useTicketsData} from "../hooks/useTicketsData";
import {useSeatSelection} from "../hooks/useSeatSelection";

const SEATS_PER_ROW = 5;

export const Tickets = () => {
    const {eventId} = useParams<{ eventId: string }>();
    const { tickets, event, loading } = useTicketsData(eventId);
    const { selectedSeats, proceeding, handleSeatClick, proceedToPurchase } = useSeatSelection({ eventId, tickets, event });

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
                                    Total: £{calculateTotalPrice(tickets, selectedSeats).toFixed(2)}
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
