import { describe, it } from "vitest";
import {
    should_display_venue_name_when_venue_exists,
    should_display_unknown_venue_when_venue_does_not_exist,
    should_display_unknown_venue_when_venues_array_is_empty,
    should_display_venue_name_for_different_venues
} from "./VenueDisplay.steps.ts";

describe("VenueDisplay", () => {
    it("should display venue name when venue exists", should_display_venue_name_when_venue_exists);
    it("should display 'Unknown Venue' when venue does not exist", should_display_unknown_venue_when_venue_does_not_exist);
    it("should display 'Unknown Venue' when venues array is empty", should_display_unknown_venue_when_venues_array_is_empty);
    it("should display venue name for different venues", should_display_venue_name_for_different_venues);
});
