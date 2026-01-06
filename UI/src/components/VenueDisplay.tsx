import { type Venue } from "../domain/venue";

type VenueDisplayProps = {
    venues: Venue[];
    venueId: string;
};

export const VenueDisplay = ({ venues, venueId }: VenueDisplayProps) => {
    const venue = venues.find(v => v.Id === venueId);
    const venueName = venue ? venue.Name : "Unknown Venue";

    return <span>{venueName}</span>;
};
