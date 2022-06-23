using Faker;
using Infrastructure.Contracts;
using Microsoft.Data.Sqlite;

namespace Sports.Infrastructure.DataAccess;

/// <inheritdoc />
public sealed class DbContext : IDbContext
{
    private readonly IList<SqliteConnection> _connectionList = new List<SqliteConnection>();
    private readonly string _connectionString;

    /// <summary>
    ///     Initialise a new DbContext. Configure DI to call this
    /// </summary>
    /// <param name="connectionString">The connection string to the database</param>
    public DbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqliteConnection NewConnection()
    {
        var conn = new SqliteConnection(_connectionString);
        _connectionList.Add(conn);

        return conn;
    }

    public void Seed()
    {
        using var connection = new SqliteConnection(_connectionString);
        _connectionList.Add(connection);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
               CREATE TABLE IF NOT EXISTS events (id INTEGER PRIMARY KEY, name TEXT, sport TEXT, visible INTEGER, advertised_start_time DATETIME)
    ";

        command.ExecuteScalar();

        for (var i = 1; i < 101; i++)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText =
                "INSERT OR IGNORE INTO events(id, name, sport, visible, advertised_start_time) VALUES ($id, $name, $sport, $visible, $time)";
            insertCommand.Parameters.Add(new SqliteParameter("$id",
                                                             i));
            insertCommand.Parameters.Add(new SqliteParameter("$name",
                                                             Name.Last()));
            insertCommand.Parameters.Add(new SqliteParameter("$sport",
                                                             Company.Name()));
            insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                             RandomNumber.Next(0,
                                                                               1)));
            insertCommand.Parameters.Add(new SqliteParameter("$time",
                                                             DateTime.Now.Add(new TimeSpan(i,
                                                                                           i,
                                                                                           i))));

            insertCommand.ExecuteScalar();
        }

        connection.Close();
    }

    public void Truncate()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
              DELETE FROM events
    ";

        command.ExecuteScalar();

        connection.Close();
    }

    /// <summary>
    ///     Dispose the context and inner connections to ensure no connections are left open
    /// </summary>
    public void Dispose()
    {
        foreach (var conn in _connectionList)
        {
            conn.Close();
        }
    }
}