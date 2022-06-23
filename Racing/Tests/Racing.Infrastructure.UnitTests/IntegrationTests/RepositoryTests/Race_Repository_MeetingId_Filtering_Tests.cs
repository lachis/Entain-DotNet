using Faker;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Racing.Infrastructure.DataAccess;
using Racing.Infrastructure.Tests.Contexts;

namespace Racing.Infrastructure.Tests.IntegrationTests.RepositoryTests;

public class Race_Repository_MeetingId_Filtering_Tests : IClassFixture<Race_Repository_MeetingId_Filtering_Tests.TestDbFixture>
{
    public TestDbFixture Fixture { get; }

    public Race_Repository_MeetingId_Filtering_Tests(TestDbFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Filter_MeetingId_1_Returns_Races_Matching_MeetingId_1_Only()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(1);
        var races = raceRepository.List(listRacesRequestFilter,new ListRacesRequestOrder());

        // assert
        races.Should()
             .OnlyContain(x => x.MeetingId == 1);
    }

    [Fact]
    public void Filter_MeetingId_2_Returns_Races_Matching_MeetingId_2_Only()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(2);
        var races = raceRepository.List(listRacesRequestFilter,new ListRacesRequestOrder());

        // assert
        // assert
        races.Should()
             .OnlyContain(x => x.MeetingId == 2);
    }

    [Fact]
    public void Filter_MeetingIds_1_2_Returns_All_Races()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(1);
        listRacesRequestFilter.MeetingIds.Add(2);
        var races = raceRepository.List(listRacesRequestFilter,new ListRacesRequestOrder());

        // assert
        races.Should()
             .HaveCount(100);

        races.Should()
             .Contain(x => x.MeetingId == 1 || x.MeetingId == 2);
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