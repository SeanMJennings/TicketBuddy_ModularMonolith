using Controllers.Events.Requests;

namespace Testing.TestData;

public static class VenueTestData
{
    public static readonly VenuePayload FirstDirectArena =
        new("First Direct Arena", "Arena Way", "Leeds", "LS2 8BY", 50);

    public static readonly VenuePayload OldTrafford =
        new("Old Trafford", "Sir Matt Busby Way", "Manchester", "M16 0RA", 45);

    public static readonly VenuePayload PrincipalityStadium =
        new("Principality Stadium", "Westgate Street", "Cardiff", "CF10 1NS", 40);

    public static IEnumerable<VenuePayload> StandardVenues()
    {
        yield return FirstDirectArena;
        yield return OldTrafford;
        yield return PrincipalityStadium;
    }
}
