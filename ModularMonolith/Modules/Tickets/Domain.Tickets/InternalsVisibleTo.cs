using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Infrastructure.Tickets")]
[assembly: InternalsVisibleTo("Testing.Unit.Tickets")]
// using this to allow EF Core access to aggregate entities for navigation whilst keeping them internal to the domain