using Faker;
using Microsoft.Data.Sqlite;
using Racing.Infrastructure.DataAccess;
using Racing.Infrastructure.Tests.Contexts;

namespace Racing.Infrastructure.Tests.IntegrationTests.RepositoryTests;

public class Race_Repository_Visible_Races_Filtering_Tests : IClassFixture<Race_Repository_Visible_Races_Filtering_Tests.TestDbFixture>
{
    public TestDbFixture Fixture { get; }

    public Race_Repository_Visible_Races_Filtering_Tests(TestDbFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Filter_Visible_Races_True_Returns_Only_Visible_Races()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.OnlyVisibleRaces = true;
        var races = raceRepository.List(listRacesRequestFilter);

        // assert
        Assert.All(races,
                   r => { Assert.True(r.Visible); });

        Assert.DoesNotContain(races,
                              r => !r.Visible);
    }

    [Fact]
    public void Filter_Visible_Races_False_Returns_Both_Visible_And_Non_Visible_Races()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.OnlyVisibleRaces = false;
        var races = raceRepository.List(listRacesRequestFilter);

        // assert
        Assert.True(100 == races.Count);

        Assert.Contains(races,
                        r => r.Visible);
        Assert.Contains(races,
                        r => !r.Visible);
    }

    [Fact]
    public void Filter_MeetingId_1_Only_Visible_Races_Returns_Only_Visible_Races_With_MeetingId_24()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(1);
        listRacesRequestFilter.OnlyVisibleRaces = true;
        var races = raceRepository.List(listRacesRequestFilter);

        // assert
        Assert.True(24 == races.Count);

        Assert.All(races,
                   r =>
                   {
                       Assert.Equal(1,
                                    r.MeetingId);
                       Assert.True(r.Visible);
                   });
    }

    [Fact]
    public void Filter_MeetingId_1_Only_Visible_Races_False_Returns_Races_With_MeetingId_1_AnyVisibility()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(1);
        listRacesRequestFilter.OnlyVisibleRaces = false;
        var races = raceRepository.List(listRacesRequestFilter);

        // assert
        Assert.All(races,
                   r =>
                   {
                       Assert.Equal(1,
                                    r.MeetingId);
                   });

        Assert.DoesNotContain(races,
                              r => r.MeetingId == 2);
        Assert.Contains(races,
                        r => r.Visible);
        Assert.Contains(races,
                        r => !r.Visible);
    }

    [Fact]
    public void Filter_MeetingIds_1_2_Only_Visible_Races_Returns_Races_Matching_MeetingId_1_2_Visible_Only()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(1);
        listRacesRequestFilter.MeetingIds.Add(2);
        listRacesRequestFilter.OnlyVisibleRaces = true;
        var races = raceRepository.List(listRacesRequestFilter);

        // assert
        Assert.Contains(races,
                        r => r.MeetingId == 2);
        Assert.Contains(races,
                        r => r.MeetingId == 1);
        Assert.DoesNotContain(races,
                              r => !r.Visible);
    }

    [Fact]
    public void Filter_MeetingIds_1_2_Only_Visible_Races_False_Returns_100_Races()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(1);
        listRacesRequestFilter.MeetingIds.Add(2);
        listRacesRequestFilter.OnlyVisibleRaces = false;
        var races = raceRepository.List(listRacesRequestFilter);

        // assert
        Assert.Contains(races,
                        r => r.MeetingId == 2);
        Assert.Contains(races,
                        r => r.MeetingId == 1);
        Assert.Contains(races,
                              r => !r.Visible);
        Assert.Contains(races,
                        r => r.Visible);
    }

    [Fact]
    public void Filter_MeetingIds_1_No_Filter_For_Visible_Returns_50_Races()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(1);
        var races = raceRepository.List(listRacesRequestFilter);

        // assert
        Assert.Contains(races,
                        r => r.MeetingId == 1);
        Assert.Contains(races,
                        r => !r.Visible);
        Assert.Contains(races,
                        r => r.Visible);
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
               CREATE TABLE IF NOT EXISTS races (id INTEGER PRIMARY KEY, meeting_id INTEGER, name TEXT, number INTEGER, visible INTEGER, advertised_start_time DATETIME)
    ";

            command.ExecuteScalar();

            for (var i = 0; i < 100; i++)
            {
                var insertCommand = SqlConnection.CreateCommand();
                insertCommand.CommandText =
                    "INSERT OR IGNORE INTO races(id, meeting_id, name, number, visible, advertised_start_time) VALUES ($id, $meetingid, $name, $number, $visible, $time)";
                insertCommand.Parameters.Add(new SqliteParameter("$id",
                                                                 i));

                if (i < 50)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$meetingid",
                                                                     1));
                }
                else
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$meetingid",
                                                                     2));
                }


                insertCommand.Parameters.Add(new SqliteParameter("$name",
                                                                 Name.Last()));
                insertCommand.Parameters.Add(new SqliteParameter("$number",
                                                                 RandomNumber.Next(1,
                                                                                   12)));

                if (i <= 25)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                     value: 0));
                }
                else if (i is > 25 and <= 50 or > 50 and <= 75)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                     1));
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