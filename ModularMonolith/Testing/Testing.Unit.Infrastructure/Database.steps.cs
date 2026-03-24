using Infrastructure.Queries;

namespace Infrastructure;

public partial class DatabaseSpecs
{
    private string? connectionString;

    private void a_null_connection_string() => connectionString = null;
    private void an_empty_connection_string() => connectionString = string.Empty;

    private void creating_a_database() => _ = new Database(connectionString!);
}