using BDD;
using Npgsql;

namespace Component;

public class TruncateDbSpecification : AsyncSpecification
{
    protected static async Task Truncate(string connectionString)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        var existingSchemas = new List<string>();
        const string schemasQuery = """
                                        SELECT schema_name 
                                        FROM information_schema.schemata 
                                        WHERE schema_name NOT IN ('pg_catalog', 'information_schema')
                                    """;

        await using (var schemaCommand = new NpgsqlCommand(schemasQuery, connection))
        await using (var schemaReader = await schemaCommand.ExecuteReaderAsync())
        {
            while (await schemaReader.ReadAsync())
            {
                existingSchemas.Add(schemaReader.GetString(0));
            }
        }

        if (existingSchemas.Count == 0) return;
        
        var schemasClause = string.Join(", ", existingSchemas.Select(s => $"'{s}'"));
        var tablesToTruncateQuery = $"""
                                         SELECT table_schema || '.' || table_name AS qualified_table
                                         FROM information_schema.tables
                                         WHERE table_schema IN ({schemasClause})
                                         AND table_name NOT LIKE '%schemaversions'
                                         AND table_name NOT LIKE '%EventVenues'
                                         AND table_type = 'BASE TABLE'
                                     """;
            
        var tablesToTruncate = new List<string>();
        await using (var command = new NpgsqlCommand(tablesToTruncateQuery, connection))
        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                tablesToTruncate.Add(reader.GetString(0));
            }
        }

        if (tablesToTruncate.Count == 0) return;

        await using (var disableConstraintsCmd = new NpgsqlCommand("SET session_replication_role = 'replica';", connection))
        {
            await disableConstraintsCmd.ExecuteNonQueryAsync();
        }

        tablesToTruncate = tablesToTruncate
            .Select(t => string.Join(".", t.Split('.').Select(part => $"\"{part}\"")))
            .ToList();
        
        foreach (var table in tablesToTruncate)
        {
            await using var truncateCommand = new NpgsqlCommand($"TRUNCATE TABLE {table} CASCADE;", connection);
            await truncateCommand.ExecuteNonQueryAsync();
        }

        await using (var enableConstraintsCmd = new NpgsqlCommand("SET session_replication_role = 'origin';", connection))
        {
            await enableConstraintsCmd.ExecuteNonQueryAsync();
        }
    }
}