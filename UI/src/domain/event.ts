﻿export interface Event {
    Id: string;
    EventName: string;
    Date: Date;
    Venue : Venue;
}

export interface EventPayload {
    EventName: string;
    Date: Date;
    Venue : Venue;
}

export enum Venue {
    O2ArenaLondon = "02ArenaLondon",
    WembleyStadiumLondon = "WembleyStadiumLondon",
    ManchesterArena = "ManchesterArena",
    UtilitaArenaBirmingham = "UtilitaArenaBirmingham",
    SSEHydroGlasgow = "SSSHydroGlasgow",
    FirstDirectArenaLeeds = "FirstDirectArenaLeeds",
    PrincipalityStadiumCardiff = "PrincipalityStadiumCardiff",
    EmiratesOldTraffordManchester = "EmiratesOldTraffordManchester",
    RoyalAlbertHallLondon = "RoyalAlbertHallLondon",
    RoundhouseLondon = "RoundhouseLondon",
}

export const ConvertVenueToString = (venue: Venue): string => {
    switch (venue) {
        case Venue.O2ArenaLondon:
            return "O2 Arena, London";
        case Venue.WembleyStadiumLondon:
            return "Wembley Stadium, London";
        case Venue.ManchesterArena:
            return "Manchester Arena";
        case Venue.UtilitaArenaBirmingham:
            return "Utilita Arena, Birmingham";
        case Venue.SSEHydroGlasgow:
            return "SSE Hydro, Glasgow";
        case Venue.FirstDirectArenaLeeds:
            return "First Direct Arena, Leeds";
        case Venue.PrincipalityStadiumCardiff:
            return "Principality Stadium, Cardiff";
        case Venue.EmiratesOldTraffordManchester:
            return "Emirates Old Trafford, Manchester";
        case Venue.RoyalAlbertHallLondon:
            return "Royal Albert Hall, London";
        case Venue.RoundhouseLondon:
            return "Roundhouse, London";
        default:
            return "Unknown Venue";
    }
}