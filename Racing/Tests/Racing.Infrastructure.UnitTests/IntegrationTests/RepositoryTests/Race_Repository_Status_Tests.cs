using Faker;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Racing.Infrastructure.DataAccess;
using Racing.Infrastructure.Tests.Contexts;

namespace Racing.Infrastructure.Tests.IntegrationTests.RepositoryTests;

public class Race_Repository_Status_Tests : IClassFixture<Race_Repository_Status_Tests.TestDbFixture>
{
    public TestDbFixture Fixture { get; }

    public Race_Repository_Status_Tests(TestDbFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void List_No_Filter_Returns_Races_50_Open_50_Closed()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act

        var races = raceRepository.List(new ListRacesRequestFilter(),
                                        new ListRacesRequestOrder());

        // assert
        races.Should()
             .Contain(x => x.Status == "OPEN");

        races.Should()
             .Contain(x => x.Status == "CLOSED");

        races.Where(x => x.Status == "OPEN")
             .ToList()
             .Should()
             .HaveCount(50);

        races.Where(x => x.Status == "CLOSED")
             .ToList()
             .Should()
             .HaveCount(50);
    }

    [Fact]
    public void List_With_Meeting_Ids_2_3_Returns_Races_25_Open_25_Closed()
    {
        // arrange
        Fixture.DbContext.Seed();
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act

        var races = raceRepository.List(new ListRacesRequestFilter
                                        {
                                            MeetingIds =
                                            {
                                                2,
                                                3
                                            }
                                        },
                                        new ListRacesRequestOrder());

        // assert
        races.Should()
             .Contain(x => x.Status == "OPEN");

        races.Should()
             .Contain(x => x.Status == "CLOSED");

        races.Where(x => x.Status == "OPEN")
             .ToList()
             .Should()
             .HaveCount(25);

        races.Where(x => x.Status == "CLOSED")
             .ToList()
             .Should()
             .HaveCount(25);
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

                if (i % 2 == 0)
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$time",
                                                                     DateTime.Now.Subtract(new TimeSpan(30,
                                                                                                        30,
                                                                                                        30))));
                }
                else
                {
                    insertCommand.Parameters.Add(new SqliteParameter("$time",
                                                                     DateTime.Now.Add(new TimeSpan(30,
                                                                                                   30,
                                                                                                   30))));
                }

                insertCommand.ExecuteScalar();
            }
        }

        public override void Truncate()
        {
            // do nothing
        }
    }
}