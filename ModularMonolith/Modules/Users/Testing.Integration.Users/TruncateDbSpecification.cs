using BDD;
using Npgsql;

namespace Integration;

public class TruncateDbSpecification : Specification
{
    protected static void Truncate(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        var existingSchemas = new List<string>();
        const string schemasQuery = """
                                        SELECT schema_name 
                                        FROM information_schema.schemata 
                                        WHERE schema_name NOT IN ('pg_catalog', 'information_schema')
                                    """;
            
        using (var schemaCommand = new NpgsqlCommand(schemasQuery, connection))
        using (var schemaReader = schemaCommand.ExecuteReader())
        {
            while (schemaReader.Read())
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
        using (var command = new NpgsqlCommand(tablesToTruncateQuery, connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                tablesToTruncate.Add(reader.GetString(0));
            }
        }

        if (tablesToTruncate.Count == 0) return;
        
        using (var disableConstraintsCmd = new NpgsqlCommand("SET session_replication_role = 'replica';", connection))
        {
            disableConstraintsCmd.ExecuteNonQuery();
        }

        tablesToTruncate = tablesToTruncate
            .Select(t => string.Join(".", t.Split('.').Select(part => $"\"{part}\"")))
            .ToList();
        
        foreach (var table in tablesToTruncate)
        {
            using var truncateCommand = new NpgsqlCommand($"TRUNCATE TABLE {table} CASCADE;", connection);
            truncateCommand.ExecuteNonQuery();
        }

        using (var enableConstraintsCmd = new NpgsqlCommand("SET session_replication_role = 'origin';", connection))
        {
            enableConstraintsCmd.ExecuteNonQuery();
        }
    }
}