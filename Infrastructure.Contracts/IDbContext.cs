using Microsoft.Data.Sqlite;
using Racing;

namespace Infrastructure.Contracts;

public interface IDbContext : IDisposable
{
    /// <summary>
    /// Get a new connection to the database.
    /// </summary>
    /// <returns>A Sqlite Database Connection</returns>
    SqliteConnection NewConnection();

    /// <summary>
    /// Generate the races table and randomly populate 100 records.
    /// </summary>
    void Seed();

    /// <summary>
    /// Will truncate the data in the table so that re-seeding may take place.
    /// </summary>
    void Truncate();
}

public interface IRaceRepository
{
    /// <summary>
    /// Retrieve a readonly collection of Races based on an input filter.
    /// </summary>
    /// <param name="filter">Instructs the repository on how to filter the races.</param>
    /// <returns>A read only collection of Race</returns>
    IReadOnlyCollection<Race> List(ListRacesRequestFilter filter);
}