import { type Venue } from "../domain/venue";

type VenueDisplayProps = {
    venues: Venue[];
    venueId: string;
};

export const VenueDisplay = ({ venues, venueId }: VenueDisplayProps) => {
    const venue = venues.find(v => v.Id === venueId);

    if (!venue) {
        return <span>Unknown Venue</span>;
    }

    const address = `${venue.Address.Street}, ${venue.Address.City}, ${venue.Address.Postcode}`;

    return (
        <span>{venue.Name}<br />{address}</span>
    );
};
