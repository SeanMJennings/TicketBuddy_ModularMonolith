import { render, screen, cleanup, type RenderResult } from "@testing-library/react";
import { VenueDisplay } from "../VenueDisplay";
import { type Venue } from "../../domain/venue";

let renderedComponent: RenderResult;

export function renderVenueDisplay(venues: Venue[], venueId: string) {
    renderedComponent = render(<VenueDisplay venues={venues} venueId={venueId} />);
    return renderedComponent;
}

export function unmountVenueDisplay() {
    cleanup();
}

export function getDisplayedText(text: string) {
    return screen.queryByText(text);
}

export function venueNameIsDisplayed(venueName: string): boolean {
    return getDisplayedText(venueName) !== null;
}

export function unknownVenueIsDisplayed(): boolean {
    return getDisplayedText("Unknown Venue") !== null;
}
