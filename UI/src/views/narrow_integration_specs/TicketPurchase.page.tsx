import { render, type RenderResult } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { vi } from "vitest";
import { userEvent } from "@testing-library/user-event";
import { Main } from "../../App.tsx";

vi.mock("../Tickets", () => {
  return {
    Tickets: () => {
      return <div>I am the mocked tickets page</div>;
    }
  }
});

let renderedComponent: RenderResult;

export function renderTicketPurchase(eventId: string, selectedTickets: any[] = [], eventData: any = null) {
  const initialEntries = [{ pathname: `/tickets/${eventId}/purchase`, state: { selectedTickets, event: eventData }}];

  renderedComponent = render(
    <MemoryRouter initialEntries={initialEntries}>
      <Main />
    </MemoryRouter>
  );

  return renderedComponent;
}

export function unmountTicketPurchase() {
  renderedComponent?.unmount();
}

export function backButtonIsRendered() {
  return elements.backButton() !== null;
}

export function titleIsRendered() {
  return elements.title() !== null;
}

export function purchaseButtonIsRendered() {
  return elements.purchaseButton() !== null;
}

export function clickBackButton() {
  return userEvent.click(elements.backButton()!);
}

export function clickPurchaseButton() {
  return userEvent.click(elements.purchaseButton()!);
}

export function seatsAreDisplayed() {
  return elements.seatItems().length > 0;
}

export function totalIsDisplayed() {
  return elements.totalSection() !== null;
}

export function ticketsPageIsRendered() {
  return elements.mockedTicketsPage() !== null;
}

export const elements = {
  title: () => renderedComponent.getByRole('heading', { name: /Ticket Purchase/i }),
  backButton: () => renderedComponent.queryByText('Back to Seat Selection'),
  purchaseButton: () => renderedComponent.getByText('Complete Purchase'),
  mockedTicketsPage: () => renderedComponent.getByText('I am the mocked tickets page'),
  seatItems: () => renderedComponent.container.querySelectorAll('[data-testid="seat-item"]'),
  totalSection: () => renderedComponent.container.querySelector('[data-testid="total-section"]'),
};
