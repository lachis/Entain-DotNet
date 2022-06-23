using Faker;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Sports.Infrastructure.DataAccess;
using Sports.Infrastructure.Tests.Contexts;

namespace Sports.Infrastructure.Tests.IntegrationTests.RepositoryTests;

public class Event_Repository_Get_Tests : IClassFixture<Event_Repository_Get_Tests.TestDbFixture>
{
     public TestDbFixture Fixture { get; }

    public Event_Repository_Get_Tests(TestDbFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Get_By_Id_5_Returns_Event_Id_5()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var @event = sportsRepository.Get(5);

        // assert
        @event.Should()
            .Match<Event>(x=>x.Id == 5);
    }

    [Fact]
    public void Get_By_Id_85_Returns_Event_Id_85()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var @event = sportsRepository.Get(85);

        // assert
        @event.Should()
            .Match<Event>(x=>x.Id == 85);
    }

    [Fact]
    public void Get_By_Id_200_Returns_Null()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var @event = sportsRepository.Get(200);

        // assert
        @event.Should()
            .BeNull();
    }

    public class TestDbFixture : IDisposable
    {
        public TestDbContext DbContext { get; }

        public TestDbFixture()
        {
            DbContext = new TestDbContext();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public SqliteConnection GetConnection()
        {
            return DbContext.NewConnection();
        }
    }

    public class TestDbContext : InMemoryTestDbContext
    {
        public override void Seed()
        {
            SqlConnection.Open();
            var command = SqlConnection.CreateCommand();
            command.CommandText = @"
               CREATE TABLE IF NOT EXISTS events (id INTEGER PRIMARY KEY, name TEXT, sport TEXT, visible INTEGER, advertised_start_time DATETIME)
    ";


            command.ExecuteScalar();

            for (var i = 0; i < 100; i++)
            {
                var insertCommand = SqlConnection.CreateCommand();
                insertCommand.CommandText =
                    "INSERT OR IGNORE INTO events(id, name, sport, visible, advertised_start_time) VALUES ($id, $name, $sport, $visible, $time)";
                insertCommand.Parameters.Add(new SqliteParameter("$id",
                                                                 i));

                
                insertCommand.Parameters.Add(new SqliteParameter("$name",
                                                                 Name.Last()));

                if (i < 50)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$sport",
                                                                     value: "Cricket"));
                }
                else
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$sport",
                                                                     value: "Football"));
                }

                insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                 RandomNumber.Next(0,
                                                                                   1)));
                insertCommand.Parameters.Add(new SqliteParameter("$time",
                                                                 DateTime.Now.Add(new TimeSpan(i,
                                                                                               i,
                                                                                               i))));

                insertCommand.ExecuteScalar();
            }
        }

        public override void Truncate()
        {
            // do nothing
        }
    }
}