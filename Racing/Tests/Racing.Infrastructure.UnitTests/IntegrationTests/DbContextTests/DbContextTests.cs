using Racing.Infrastructure.Tests.Fixtures;

namespace Racing.Infrastructure.Tests.IntegrationTests.DbContextTests;

[Collection("Database_Seeding_Cleaning")]
public class DbContextTests : IClassFixture<DbContextFixture>
{
    public DbContextFixture Fixture { get; }

    public DbContextTests(DbContextFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Seed_Generates_100_Race_Records_Successfully()
    {
        // arrange
        using var connection = Fixture.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
       SELECT 
				count(*) 
			FROM races
    ";

        // act
        var count = 0;
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                count = reader.GetInt32(0);
            }
        }

        // assert
        Assert.Equal(100,
                     count);
    }

    [Fact]
    public void Seed_Is_Idempotent()
    {
        // arrange
        using var connection = Fixture.GetConnection();
        Fixture.DbContext.Seed();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
       SELECT 
				count(*) 
			FROM races
    ";

        // act
        var count = 0;
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                count = reader.GetInt32(0);
            }
        }

        // assert 
        Assert.Equal(100,
                     count);
    }

    [Fact]
    public void Truncate_Will_Remove_All_Data()
    {
        // arrange
        using var connection = Fixture.GetConnection();
        Fixture.DbContext.Truncate();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
       SELECT 
				count(*) 
			FROM races
    ";

        // act
        var count = 0;
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                count = reader.GetInt32(0);
            }
        }

        // assert 
        Assert.Equal(0,
                     count);
    }
}