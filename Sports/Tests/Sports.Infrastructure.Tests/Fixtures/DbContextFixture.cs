using Microsoft.Data.Sqlite;
using Sports.Infrastructure.DataAccess;

namespace Sports.Infrastructure.Tests.Fixtures;

public class DbContextFixture : IDisposable
{
    private const string ConnectionString = "Data Source=sports.db";

    public DbContext DbContext { get; }

    public DbContextFixture()
    {
        DbContext = new DbContext(ConnectionString);
        DbContext.Seed();
    }

    public void Dispose()
    {
        DbContext.Truncate();
        DbContext.Dispose();
    }

    public SqliteConnection GetConnection()
    {
        return DbContext.NewConnection();
    }
}