using Faker;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Racing.Infrastructure.DataAccess;
using Racing.Infrastructure.Tests.Contexts;

namespace Racing.Infrastructure.Tests.IntegrationTests.RepositoryTests;

public class Race_Repository_Filtering_Ordering_Complex_Tests : IClassFixture<Race_Repository_Filtering_Ordering_Complex_Tests.TestDbFixture>
{
    public TestDbFixture Fixture { get; }

    public Race_Repository_Filtering_Ordering_Complex_Tests(TestDbFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Filter_MeetingId_1_3_5_Does_Not_Return_MeetingId_2_4_And_Is_Ordered_By_StartTime()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(1);
        listRacesRequestFilter.MeetingIds.Add(3);
        listRacesRequestFilter.MeetingIds.Add(5);

        var listRacesRequestOrder = new ListRacesRequestOrder
                                    {
                                        Field = "advertised_start_time"
                                    };
        var races = raceRepository.List(listRacesRequestFilter, listRacesRequestOrder);

        // assert
        races.Should()
             .BeInAscendingOrder(x => x.AdvertisedStartTime);

        races.Should()
             .Contain(x => x.MeetingId == 1 || x.MeetingId == 3 || x.MeetingId == 5);
       
        races.Should()
             .NotContain(x => x.MeetingId == 2);

        races.Should()
             .NotContain(x => x.MeetingId == 4);

        races.Should()
             .Contain(x => !x.Visible);

        races.Should()
             .Contain(x => x.Visible);
    }

    [Fact]
    public void Filter_MeetingId_1_3_5_With_Visible_True_Does_Not_Return_MeetingId_2_4_Or_NonVisible_Returns_OrderedBy_Meeting_Id()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(1);
        listRacesRequestFilter.MeetingIds.Add(3);
        listRacesRequestFilter.MeetingIds.Add(5);
        listRacesRequestFilter.OnlyVisibleRaces = true;
        var listRacesRequestOrder = new ListRacesRequestOrder
                                    {
                                        Field = "meeting_id"
                                    };
        var races = raceRepository.List(listRacesRequestFilter,listRacesRequestOrder);

        // assert
        races.Should()
             .BeInAscendingOrder(x => x.MeetingId);

        races.Should()
             .Contain(x => x.MeetingId == 1 || x.MeetingId == 3 || x.MeetingId == 5);

        races.Should()
             .NotContain(x => x.MeetingId == 2);

        races.Should()
             .NotContain(x => x.MeetingId == 4);

        races.Should()
             .NotContain(x => !x.Visible);
    }

    [Fact]
    public void Filter_MeetingId_5_With_Visible_True_Only_Returns_MeetingId_5_Visible_And_OrderedBy_Meeting_Id()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);
    
        // act
        var listRacesRequestFilter = new ListRacesRequestFilter();
        listRacesRequestFilter.MeetingIds.Add(5);
        listRacesRequestFilter.OnlyVisibleRaces = true;
        var listRacesRequestOrder = new ListRacesRequestOrder
                                    {
                                        Field = "meeting_id"
                                    };
        var races = raceRepository.List(listRacesRequestFilter,listRacesRequestOrder);

        // assert
        races.Should()
             .BeInAscendingOrder(x => x.MeetingId);

        races.Should()
             .OnlyContain(x => x.Visible);

        races.Should()
             .OnlyContain(x => x.MeetingId == 5);
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

            for (var i = 1; i < 1010; i++)
            {
                var insertCommand = SqlConnection.CreateCommand();
                insertCommand.CommandText =
                    "INSERT OR IGNORE INTO races(id, meeting_id, name, number, visible, advertised_start_time) VALUES ($id, $meetingid, $name, $number, $visible, $time)";
                insertCommand.Parameters.Add(new SqliteParameter("$id",
                                                                 i));

                if (i <= 200)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$meetingid",
                                                                     value: 1));
                }
                else if (i <= 400)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$meetingid",
                                                                     value: 2));
                }
                else if (i <= 600)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$meetingid",
                                                                     value: 3));
                }
                else if (i <= 800)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$meetingid",
                                                                     value: 4));
                }
                else if (i <= 1000)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$meetingid",
                                                                     value: 5));
                }
                else if (i <= 1005)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$meetingid",
                                                                     value: 6));
                }
                else 
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$meetingid",
                                                                     value: 7));
                }

                insertCommand.Parameters.Add(new SqliteParameter("$name",
                                                                 Name.Last()));
                insertCommand.Parameters.Add(new SqliteParameter("$number",
                                                                 RandomNumber.Next(1,
                                                                                   12)));
                if (i is > 1005 and <= 1010)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$visible",
                                                                     value: 1));
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
                                                                     value: 1));
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