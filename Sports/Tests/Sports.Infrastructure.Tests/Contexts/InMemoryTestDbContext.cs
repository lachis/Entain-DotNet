using Infrastructure.Contracts;
using Microsoft.Data.Sqlite;

namespace Sports.Infrastructure.Tests.Contexts;

public abstract class InMemoryTestDbContext : IDbContext
{
    protected readonly SqliteConnection SqlConnection;

    /// <summary>
    ///     Initialise a new in-memory TestDbContext. Use this context in place of DbContext for testing.
    /// </summary>
    protected InMemoryTestDbContext()
    {
        const string connectionString = "Data Source=:memory:";
        SqlConnection = new SqliteConnection(connectionString);
    }

    public void Dispose()
    {
        SqlConnection.Dispose();
    }

    public SqliteConnection NewConnection()
    {
        return SqlConnection;
    }

    /// <summary>
    ///     Implementers should specify their own Seed methods depending on the test case.
    /// </summary>
    public abstract void Seed();

    /// <summary>
    ///     Implementers should specify their own Truncate methods depending on the test case.
    /// </summary>
    public abstract void Truncate();
}


