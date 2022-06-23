using Microsoft.Data.Sqlite;
using Racing.Infrastructure.DataAccess;

namespace Racing.Infrastructure.Tests.Fixtures;

public class DbContextFixture : IDisposable
{
    private const string ConnectionString = "Data Source=racing.db";

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