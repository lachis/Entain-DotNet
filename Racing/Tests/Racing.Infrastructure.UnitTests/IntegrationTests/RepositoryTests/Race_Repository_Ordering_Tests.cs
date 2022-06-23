using Faker;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Racing.Infrastructure.DataAccess;
using Racing.Infrastructure.Tests.Contexts;

namespace Racing.Infrastructure.Tests.IntegrationTests.RepositoryTests;

public class Race_Repository_Ordering_Tests : IClassFixture<Race_Repository_Ordering_Tests.TestDbFixture>
{
    public TestDbFixture Fixture { get; }

    public Race_Repository_Ordering_Tests(TestDbFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void List_OrderBy_AdvertisedStartTime_Ascending_Returns_Ordered_Results()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);
      
        var races = raceRepository.List(new ListRacesRequestFilter(), new ListRacesRequestOrder()
                                                                      {
                                                                          Field = "advertised_start_time"
                                                                      });

        // assert
        races.Should()
             .BeInAscendingOrder(x => x.AdvertisedStartTime);
    }

    [Fact]
    public void List_OrderBy_AdvertisedStartTime_Ascending_With_UPPERCASE_Returns_Ordered_Results()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);
      
        var races = raceRepository.List(new ListRacesRequestFilter(), new ListRacesRequestOrder()
                                                                      {
                                                                          Field = "ADVERTISED_start_time"
                                                                      });

        // assert
        races.Should()
             .BeInAscendingOrder(x => x.AdvertisedStartTime);
    }

    [Fact]
    public void List_OrderBy_Incorrect_Spelling_AdvertisedStartTime_Ascending_Does_Not_Return_Ordered_Results()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);
      
        var races = raceRepository.List(new ListRacesRequestFilter(), new ListRacesRequestOrder()
                                                                      {
                                                                          Field = "nme"
                                                                      });

        // assert
        races.Should()
             .NotBeInAscendingOrder(x => x.Name);
    }

    [Fact]
    public void List_OrderBy_Name_Ascending_Returns_Ordered_Results()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);
      
        var races = raceRepository.List(new ListRacesRequestFilter(), new ListRacesRequestOrder()
                                                                      {
                                                                          Field = "name"
                                                                      });

        // assert
        races.Should()
             .BeInAscendingOrder(x => x.Name);
    }

    [Fact]
    public void List_OrderBy_Number_Ascending_Returns_Ordered_Results()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);
      
        var races = raceRepository.List(new ListRacesRequestFilter(), new ListRacesRequestOrder()
                                                                      {
                                                                          Field = "number"
                                                                      });

        // assert
        races.Should()
             .BeInAscendingOrder(x => x.Number);
    }

    [Fact]
    public void List_OrderBy_MeetingId_Ascending_Returns_Ordered_Results()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);
      
        var races = raceRepository.List(new ListRacesRequestFilter(), new ListRacesRequestOrder()
                                                                      {
                                                                          Field = "meeting_id"
                                                                      });

        // assert
        races.Should()
             .BeInAscendingOrder(x => x.MeetingId);
    }

    [Fact]
    public void List_OrderBy_Visible_Ascending_Does_Not_Return_Ordered_Results()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);
      
        var races = raceRepository.List(new ListRacesRequestFilter(), new ListRacesRequestOrder()
                                                                      {
                                                                          Field = "visible"
                                                                      });

        // assert
        races.Should()
             .NotBeInAscendingOrder(x => x.Visible);
    }

    [Fact]
    public void List_OrderBy_Id_Ascending_Returns_Ordered_Results()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);
      
        var races = raceRepository.List(new ListRacesRequestFilter(), new ListRacesRequestOrder()
                                                                      {
                                                                          Field = "id"
                                                                      });

        // assert
        races.Should()
             .BeInAscendingOrder(x => x.Id);
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