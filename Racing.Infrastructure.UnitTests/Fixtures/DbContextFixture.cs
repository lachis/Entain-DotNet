using Microsoft.Data.Sqlite;
using Racing.Infrastructure.DataAccess;

namespace Racing.Infrastructure.UnitTests.Fixtures;

public class DbContextFixture : IDisposable
{
    private const string ConnectionString = "Data Source=racing.db";

    public DbContextFixture()
    {
        DbContext = new DbContext(ConnectionString);
        DbContext.Seed();
    }

    public DbContext DbContext { get; }

    public SqliteConnection GetConnection() => DbContext.NewConnection();

    public void Dispose()
    {
        DbContext.Truncate();
        DbContext.Dispose();
    }
}

