using Faker;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Sports.Infrastructure.DataAccess;
using Sports.Infrastructure.Tests.Contexts;

namespace Sports.Infrastructure.Tests.IntegrationTests.RepositoryTests;

public class Event_Repository_Sports_Filtering_Tests : IClassFixture<Event_Repository_Sports_Filtering_Tests.TestDbFixture>
{
    public TestDbFixture Fixture { get; }

    public Event_Repository_Sports_Filtering_Tests(TestDbFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Filter_Sport_Cricket_Returns_Events_Matching_Cricket_Only()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Cricket");
        var events = sportsRepository.List(listEventsRequestFilter,new ListEventsRequestOrder());

        // assert
        events.Should()
             .OnlyContain(x => x.Sport == "Cricket");

        events.Should()
              .HaveCount(50);
    }

    [Fact]
    public void Filter_Sport_Soccer_Returns_Events_Matching_Soccer_Only_Empty()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Soccer");
        var events = sportsRepository.List(listEventsRequestFilter,new ListEventsRequestOrder());

        // assert
        events.Should()
              .BeEmpty();
    }

    [Fact]
    public void Filter_Sport_Cricket_Soccer_Returns_50_Events()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Cricket");
        listEventsRequestFilter.Sport.Add("Soccer");
        var events = sportsRepository.List(listEventsRequestFilter,new ListEventsRequestOrder());

        // assert
        events.Should()
             .HaveCount(50);

        events.Should()
             .Contain(x => x.Sport == "Cricket");

        events.Should()
              .NotContain(x => x.Sport == "Soccer");
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