namespace Dataseeder.SeedData;

internal static class VenueSeedData
{
    internal static readonly IReadOnlyList<VenueData> Venues =
    [
        new("FirstDirectArena", "First Direct Arena", "Arena Way", "Leeds", "LS2 8BY", 50u),
        new("OldTrafford", "Old Trafford", "Sir Matt Busby Way", "Manchester", "M16 0RA", 45u),
        new("PrincipalityStadium", "Principality Stadium", "Westgate Street", "Cardiff", "CF10 1NS", 40u),
        new("RoyalAlbertHall", "Royal Albert Hall", "Kensington Gore", "London", "SW7 2AP", 35u),
        new("O2Arena", "The O2 Arena", "Peninsula Square", "London", "SE10 0DX", 50u)
    ];
}

internal record VenueData(string Key, string Name, string Street, string City, string Postcode, uint Capacity);