using System.Data;
using Dapper;
using Domain;
using Npgsql;

namespace Infrastructure.Queries
{
    public class Database
    {
        private string ConnectionString { get; }

        public Database(string connectionString)
        {
            Validation.BasedOn(errors =>
            {
                if (string.IsNullOrEmpty(connectionString)) errors.Add("A connection string is required.");
            });

            ConnectionString = connectionString;
        }

        private static NpgsqlConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        public async Task<IEnumerable<T>> Query<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            await using var connection = CreateConnection(ConnectionString);
            return await connection.QueryAsync<T>(sql, param, commandType: commandType).ConfigureAwait(false);
        }
    }
}