import type {Moment} from "moment";

export interface Event {
    Id: string;
    EventName: string;
    StartDate: Moment;
    EndDate: Moment;
    VenueId: string;
    Price: number;
    IsSoldOut: boolean;
}

export interface EventPayload {
    EventName: string;
    StartDate: Moment;
    EndDate: Moment;
    VenueId: string;
    Price: number
}

export interface UpdateEventPayload {
    EventName: string;
    StartDate: Moment;
    EndDate: Moment;
    Price: number
}