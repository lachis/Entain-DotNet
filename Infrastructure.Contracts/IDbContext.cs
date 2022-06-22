using Microsoft.Data.Sqlite;

namespace Infrastructure.Contracts;

public interface IDbContext : IDisposable
{
    /// <summary>
    ///     Get a new connection to the database.
    /// </summary>
    /// <returns>A Sqlite Database Connection</returns>
    SqliteConnection NewConnection();

    /// <summary>
    ///     Generate the races table and randomly populate 100 records.
    /// </summary>
    void Seed();

    /// <summary>
    ///     Will truncate the data in the table so that re-seeding may take place.
    /// </summary>
    void Truncate();
}