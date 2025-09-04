﻿import { useState } from 'react';
import { useLocation, useNavigate, Link, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Button } from '../components/Button.styles';
import { BackIcon } from './EventsManagement.styles';
import { type Ticket } from '../domain/ticket';
import { type Event } from '../domain/event';
import { useUsersStore } from '../stores/users.store';
import { useShallow } from 'zustand/react/shallow';
import {
    PurchaseContainer,
    PurchaseTitle,
    PurchaseSummary,
    EventDetails,
    EventName,
    EventDate,
    EventVenue,
    TicketsList,
    TicketHeader,
    TicketItem,
    TotalSection,
    ActionBar,
    SuccessMessage, CenteredButtonContainer
} from './TicketPurchase.styles';
import {purchaseTickets} from "../api/tickets.api.ts";
import {handleError} from "../common/http.ts";
import {PageTitle} from "./Common.styles.tsx";

interface LocationState {
  selectedTickets: Ticket[];
  event: Event | null;
}

export const TicketPurchase = () => {
  const { eventId } = useParams<{ eventId: string }>();
  const location = useLocation();
  const navigate = useNavigate();
  const [purchasing, setPurchasing] = useState(false);
  const [purchaseComplete, setPurchaseComplete] = useState(false);

  const { user } = useUsersStore(useShallow((state) => ({
      user: state.user
  })));

  const state = location.state as LocationState;
  const { selectedTickets, event } = state || { selectedTickets: [], event: null };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-GB', {
      day: 'numeric',
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const calculateTotal = () => {
    if (selectedTickets.length === 0) return 0;
    return selectedTickets.reduce((total, ticket) => total + ticket.Price, 0);
  };

  const handlePurchase = async () => {
    if (!user || !eventId || selectedTickets.length === 0) {
      toast.error('Unable to process purchase. Please try again.');
      return;
    }

    setPurchasing(true);
      const ticketIds = selectedTickets.map(ticket => ticket.Id);

      purchaseTickets(eventId, {
          UserId: user.Id,
          TicketIds: ticketIds
      }).then(() => {
              setPurchaseComplete(true);
          }
      ).catch(handleError).finally(() => {
          setPurchasing(false);
      });
  };

  if (!event || selectedTickets.length === 0) {
    return (
      <PurchaseContainer>
        <ActionBar>
          <Link to={`/tickets/${eventId}`}>
            <Button>
              <BackIcon /> Back to Seat Selection
            </Button>
          </Link>
        </ActionBar>
        <PurchaseTitle>No tickets selected</PurchaseTitle>
        <p>Please select tickets before proceeding to purchase.</p>
      </PurchaseContainer>
    );
  }

  return (
    <PurchaseContainer>
      <PageTitle>Ticket Purchase</PageTitle>

        <ActionBar>
            <Link to={`/tickets/${eventId}`}>
                <Button>
                    <BackIcon /> Back to Seat Selection
                </Button>
            </Link>
        </ActionBar>

      {purchaseComplete ? (
        <SuccessMessage>
          <h2>Purchase Complete!</h2>
          <p>Thank you for your purchase. Your tickets have been reserved.</p>
          <Button onClick={() => navigate('/')}>Back to Events</Button>
        </SuccessMessage>
      ) : (
        <>
          <PurchaseSummary>
            <EventDetails>
              <EventName>{event.EventName}</EventName>
              <EventDate>{formatDate(event.StartDate.toString())} - {formatDate(event.EndDate.toString())}</EventDate>
              <EventVenue>Venue: {event.Venue}</EventVenue>
            </EventDetails>

            <TicketsList>
              <TicketHeader>Selected Tickets</TicketHeader>
              {selectedTickets.map((ticket) => (
                <TicketItem key={ticket.SeatNumber} data-testid="seat-item">
                  <span>Seat {ticket.SeatNumber}</span>
                  <span>£{ticket.Price.toFixed(2)}</span>
                </TicketItem>
              ))}
            </TicketsList>

            <TotalSection data-testid="total-section">
              <span>Total:</span>
              <span>£{calculateTotal().toFixed(2)}</span>
            </TotalSection>
          </PurchaseSummary>

            <CenteredButtonContainer>
                <Button
                    onClick={handlePurchase}
                    disabled={purchasing}
                >
                    {purchasing ? 'Processing...' : 'Complete Purchase'}
                </Button>
            </CenteredButtonContainer>
        </>
      )}
    </PurchaseContainer>
  );
};
