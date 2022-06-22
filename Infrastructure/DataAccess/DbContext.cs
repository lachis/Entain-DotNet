using Faker;
using Infrastructure.Contracts;
using Microsoft.Data.Sqlite;

namespace Racing.Infrastructure.DataAccess;

/// <inheritdoc />
public sealed class DbContext : IDbContext
{
    private readonly string _connectionString;
    private readonly IList<SqliteConnection> _connectionList = new List<SqliteConnection>();

    /// <summary>
    /// Initialise a new DbContext. Configure DI to call this
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
               CREATE TABLE IF NOT EXISTS races (id INTEGER PRIMARY KEY, meeting_id INTEGER, name TEXT, number INTEGER, visible INTEGER, advertised_start_time DATETIME)
    ";

        command.ExecuteScalar();

        for (var i = 1; i < 101; i++)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText =
                "INSERT OR IGNORE INTO races(id, meeting_id, name, number, visible, advertised_start_time) VALUES ($id, $meetingid, $name, $number, $visible, $time)";
            insertCommand.Parameters.Add(new SqliteParameter("$id",
                                                             i));
            insertCommand.Parameters.Add(new SqliteParameter("$meetingid",
                                                             RandomNumber.Next(1,
                                                                               10)));
            insertCommand.Parameters.Add(new SqliteParameter("$name",
                                                             Name.Last()));
            insertCommand.Parameters.Add(new SqliteParameter("$number",
                                                             RandomNumber.Next(1,
                                                                               12)));
            insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                             RandomNumber.Next(0,
                                                                               1)));
            insertCommand.Parameters.Add(new SqliteParameter("$time",
                                                             DateTime.Now.Add(new TimeSpan(i, i, i))));

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
              DELETE FROM races
    ";

        command.ExecuteScalar();

        connection.Close();
    }

    /// <summary>
    /// Dispose the context and inner connections to ensure no connections are left open
    /// </summary>
    public void Dispose()
    {
        foreach (var conn in _connectionList)
        {
            conn.Close();
        }
    }
}