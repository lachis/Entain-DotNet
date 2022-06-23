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

    //[Fact]
    //public void Filter_MeetingId_1_3_5_With_Visible_True_Does_Not_Return_MeetingId_2_4_Or_NonVisible()
    //{
    //    // arrange
    //    Fixture.DbContext.Seed();
    //    var raceRepository = new RaceRepository(Fixture.DbContext);
    //    var ids = new List<long>
    //              {
    //                  1,
    //                  3,
    //                  5
    //              };

    //    // act
    //    var listRacesRequestFilter = new ListRacesRequestFilter();
    //    listRacesRequestFilter.MeetingIds.Add(1);
    //    listRacesRequestFilter.MeetingIds.Add(3);
    //    listRacesRequestFilter.MeetingIds.Add(5);
    //    listRacesRequestFilter.OnlyVisibleRaces = true;
    //    var races = raceRepository.List(listRacesRequestFilter,new ListRacesRequestOrder());

    //    // assert
    //    Assert.All(races,
    //               r =>
    //               {
    //                   var contains = ids.Contains(r.MeetingId);
    //                   Assert.True(contains);
    //               });
    //    Assert.DoesNotContain(races, r => r.MeetingId == 2);
    //    Assert.DoesNotContain(races, r => r.MeetingId == 4);
    //    Assert.DoesNotContain(races, r => !r.Visible);
    //}

    //[Fact]
    //public void Filter_MeetingIds_6_With_Visible_True_Returns_No_Results()
    //{
    //    // arrange
    //    Fixture.DbContext.Seed();
    //    var raceRepository = new RaceRepository(Fixture.DbContext);

    //    // act
    //    var listRacesRequestFilter = new ListRacesRequestFilter();
    //    listRacesRequestFilter.MeetingIds.Add(6);
    //    listRacesRequestFilter.OnlyVisibleRaces = true;
    //    var races = raceRepository.List(listRacesRequestFilter,new ListRacesRequestOrder());

    //    // assert
    //    Assert.Empty(races);
    //}

    //[Fact]
    //public void Filter_MeetingIds_7_With_Visible_False_Only_Returns_Races_With_MeetingId_7_But_Only_Visible_Despite_Filter()
    //{
    //    // arrange
    //    Fixture.DbContext.Seed();
    //    var raceRepository = new RaceRepository(Fixture.DbContext);
  
    //    // act
    //    var listRacesRequestFilter = new ListRacesRequestFilter();
    //    listRacesRequestFilter.MeetingIds.Add(7);
    //    listRacesRequestFilter.OnlyVisibleRaces = false;
    //    var races = raceRepository.List(listRacesRequestFilter,new ListRacesRequestOrder());

    //    // assert
    //    Assert.DoesNotContain(races,
    //                          r => r.MeetingId != 7);

    //    Assert.Contains(races,
    //                    r => r.Visible);
    //    Assert.DoesNotContain(races, r => !r.Visible);
    //}

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