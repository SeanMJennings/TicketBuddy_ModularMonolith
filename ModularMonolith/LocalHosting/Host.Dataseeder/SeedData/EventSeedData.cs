using Domain.ValueObjects;

namespace Dataseeder.SeedData;

internal static class EventSeedData
{
    internal static IReadOnlyList<EventData> GetEvents() =>
    [
        new("Summer Rock Festival", DateTime.Now.AddDays(30), DateTime.Now.AddDays(30).AddHours(1), "FirstDirectArena", 50m),
        new("Classical Symphony", DateTime.Now.AddDays(45), DateTime.Now.AddDays(45).AddHours(1), "RoyalAlbertHall", 75m),
        new("International Football Match", DateTime.Now.AddDays(60), DateTime.Now.AddDays(60).AddHours(1), "OldTrafford", 100m),
        new("Comedy Night Special", DateTime.Now.AddDays(15), DateTime.Now.AddDays(15).AddHours(1), "O2Arena", 30m),
        new("Tech Conference", DateTime.Now.AddDays(90), DateTime.Now.AddDays(90).AddHours(1), "PrincipalityStadium", 200m),
        new("Jazz Evening", DateTime.Now.AddDays(20), DateTime.Now.AddDays(20).AddHours(1), "FirstDirectArena", 60m),
        new("Pop Concert", DateTime.Now.AddDays(25), DateTime.Now.AddDays(25).AddHours(1), "RoyalAlbertHall", 80m),
        new("Basketball Championship", DateTime.Now.AddDays(35), DateTime.Now.AddDays(35).AddHours(1), "OldTrafford", 120m),
        new("Theater Play", DateTime.Now.AddDays(40), DateTime.Now.AddDays(40).AddHours(1), "O2Arena", 45m),
        new("Business Summit", DateTime.Now.AddDays(70), DateTime.Now.AddDays(70).AddHours(1), "PrincipalityStadium", 250m)
    ];
}

internal record EventData(EventName Name, DateTime StartDate, DateTime EndDate, string VenueKey, Money Price);