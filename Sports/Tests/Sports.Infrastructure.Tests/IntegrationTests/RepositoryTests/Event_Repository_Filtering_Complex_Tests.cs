using Faker;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Sports.Infrastructure.DataAccess;
using Sports.Infrastructure.Tests.Contexts;

namespace Sports.Infrastructure.Tests.IntegrationTests.RepositoryTests;

public class Event_Repository_Filtering_Complex_Tests : IClassFixture<Event_Repository_Filtering_Complex_Tests.TestDbFixture>
{
    public TestDbFixture Fixture { get; }

    public Event_Repository_Filtering_Complex_Tests(TestDbFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Filter_Sport_Cricket_Netball_Football_Does_Not_Return_Sport_Soccer_RClimbing()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Cricket");
        listEventsRequestFilter.Sport.Add("Netball");
        listEventsRequestFilter.Sport.Add("Football");
        var events = sportsRepository.List(listEventsRequestFilter,
                                        new ListEventsRequestOrder());

        // assert
        events.Should()
             .Contain(x => x.Sport == "Cricket" || x.Sport == "Netball" || x.Sport == "Football");

        events.Should()
             .NotContain(x => x.Sport == "Soccer");

        events.Should()
             .NotContain(x => x.Sport == "Rock Climbing");

        events.Should()
             .Contain(x => !x.Visible);

        events.Should()
             .Contain(x => x.Visible);
    }

    [Fact]
    public void Filter_Sport_Cricket_Netball_Football_Does_Not_Return_Sport_Soccer_RClimbing_Or_NonVisible()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Cricket");
        listEventsRequestFilter.Sport.Add("Netball");
        listEventsRequestFilter.Sport.Add("Football");
        listEventsRequestFilter.OnlyVisibleEvents = true;
        var events = sportsRepository.List(listEventsRequestFilter,
                                        new ListEventsRequestOrder());

        // assert
        events.Should()
              .Contain(x => x.Sport == "Cricket" || x.Sport == "Netball" || x.Sport == "Football");

        events.Should()
             .NotContain(x => !x.Visible);
    }

    [Fact]
    public void Filter_Sport_Rock_Climbing_With_Visible_True_Returns_No_Results()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Rock Climbing");
        listEventsRequestFilter.OnlyVisibleEvents = true;
        var events = sportsRepository.List(listEventsRequestFilter,
                                        new ListEventsRequestOrder());

        // assert
        Assert.Empty(events);
    }

    [Fact]
    public void Filter_Sport_Hockey_With_Visible_False_Only_Returns_Events_With_Sport_Hockey_But_Only_Visible_Despite_Filter()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Hockey");
        listEventsRequestFilter.OnlyVisibleEvents = false;
        var events = sportsRepository.List(listEventsRequestFilter,
                                        new ListEventsRequestOrder());

        // assert
        Assert.DoesNotContain(events,
                              r => r.Sport != "Hockey");

        Assert.Contains(events,
                        r => r.Visible);
        Assert.DoesNotContain(events,
                              r => !r.Visible);
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
            DbContext.Dispose();
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

            for (var i = 1; i < 1010; i++)
            {
                var insertCommand = SqlConnection.CreateCommand();
                insertCommand.CommandText =
                    "INSERT OR IGNORE INTO events(id, name, sport, visible, advertised_start_time) VALUES ($id, $name, $sport, $visible, $time)";
                insertCommand.Parameters.Add(new SqliteParameter("$id",
                                                                 i));

              

                insertCommand.Parameters.Add(new SqliteParameter("$name",
                                                                 Name.Last()));
                if (i <= 200)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$sport",
                                                                     value: "Cricket"));
                }
                else if (i <= 400)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$sport",
                                                                     value: "Tennis"));
                }
                else if (i <= 600)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$sport",
                                                                     value: "Soccer"));
                }
                else if (i <= 800)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$sport",
                                                                     value: "Football"));
                }
                else if (i <= 1000)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$sport",
                                                                     value: "Netball"));
                }
                else if (i <= 1005)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$sport",
                                                                     value: "Rock Climbing"));
                }
                else
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$sport",
                                                                     value: "Hockey"));
                }

                if (i is > 1005 and <= 1010)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                     1));
                }
                else if (i >= 1000)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                     value: 0));
                }
                else if (i % 2 == 0)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                     value: 0));
                }
                else
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                     1));
                }

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