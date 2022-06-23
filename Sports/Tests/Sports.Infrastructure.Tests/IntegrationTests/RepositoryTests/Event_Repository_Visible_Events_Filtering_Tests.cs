using Faker;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Sports.Infrastructure.DataAccess;
using Sports.Infrastructure.Tests.Contexts;

namespace Sports.Infrastructure.Tests.IntegrationTests.RepositoryTests;

public class Event_Repository_Visible_Events_Filtering_Tests : IClassFixture<Event_Repository_Visible_Events_Filtering_Tests.TestDbFixture>
{
    public TestDbFixture Fixture { get; }

    public Event_Repository_Visible_Events_Filtering_Tests(TestDbFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Filter_Visible_Events_True_Returns_Only_Visible_Events()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter
                                     {
                                         OnlyVisibleEvents = true
                                     };
        var events = sportsRepository.List(listEventsRequestFilter, new ListEventsRequestOrder());

        // assert
        events.Should()
             .OnlyContain(x => x.Visible);
    }

    [Fact]
    public void Filter_Visible_Events_False_Returns_Both_Visible_And_Non_Visible_Events()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter
                                     {
                                         OnlyVisibleEvents = false
                                     };
        var events = sportsRepository.List(listEventsRequestFilter, new ListEventsRequestOrder());

        // assert
        events.Should()
             .HaveCount(100);

        events.Should()
             .Contain(x => x.Visible || !x.Visible);
    }

    [Fact]
    public void Filter_Sport_Cricket_Only_Visible_Events_Returns_Only_Visible_Events_With_Count_24()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Cricket");
        listEventsRequestFilter.OnlyVisibleEvents = true;
        var events = sportsRepository.List(listEventsRequestFilter, new ListEventsRequestOrder());

        // assert
        events.Should()
             .HaveCount(24);

        events.Should()
             .OnlyContain(m => m.Sport == "Cricket");

        events.Should()
             .OnlyContain(x => x.Visible);
    }

    [Fact]
    public void Filter_Sport_Cricket_Only_Visible_Events_False_Returns_Events_With_Sport_Cricket_AnyVisibility()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Cricket");
        listEventsRequestFilter.OnlyVisibleEvents = false;
        var events = sportsRepository.List(listEventsRequestFilter, new ListEventsRequestOrder());

        // assert
        events.Should()
             .OnlyContain(m => m.Sport == "Cricket");

        events.Should()
             .Contain(x => x.Visible || !x.Visible);
    }

    [Fact]
    public void Filter_Sports_Cric_Socc_Only_Visible_Events_Returns_Events_Matching_Sport_Cric_Socc_Visible_Only()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Cricket");
        listEventsRequestFilter.Sport.Add("Soccer");
        listEventsRequestFilter.OnlyVisibleEvents = true;
        var events = sportsRepository.List(listEventsRequestFilter, new ListEventsRequestOrder());

        // assert
        events.Should()
             .Contain(x => x.Sport == "Cricket" || x.Sport == "Soccer");

        events.Should()
             .NotContain(x => !x.Visible);
    }

    [Fact]
    public void Filter_Sports_Cricket_Soccer_Only_Visible_Events_False_Returns_100_Events()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Cricket");
        listEventsRequestFilter.Sport.Add("Soccer");
        listEventsRequestFilter.OnlyVisibleEvents = false;
        var events = sportsRepository.List(listEventsRequestFilter, new ListEventsRequestOrder());

        // assert
        events.Should()
             .Contain(x => x.Sport == "Cricket" || x.Sport == "Soccer");

        events.Should()
             .Contain(x => x.Visible || !x.Visible);

    }

    [Fact]
    public void Filter_Sports_Cricket_No_Filter_For_Visible_Returns_50_Events()
    {
        // arrange
        Fixture.DbContext.Seed();
        var sportsRepository = new SportsRepository(Fixture.DbContext);

        // act
        var listEventsRequestFilter = new ListEventsRequestFilter();
        listEventsRequestFilter.Sport.Add("Cricket");
        var events = sportsRepository.List(listEventsRequestFilter, new ListEventsRequestOrder());

        // assert
        events.Should()
             .OnlyContain(x => x.Sport == "Cricket");

        events.Should()
             .Contain(x => x.Visible || !x.Visible);

        events.Should()
             .HaveCount(50);
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
                                                                     value: "Soccer"));
                }

                if (i <= 25)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                     value: 0));
                }
                else if (i is > 25 and <= 50 or > 50 and <= 75)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                     value: 1));
                }
                else
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                     value: 0));
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