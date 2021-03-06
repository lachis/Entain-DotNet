using FluentAssertions;
using Racing.Infrastructure.DataAccess;
using Racing.Infrastructure.Tests.Fixtures;

namespace Racing.Infrastructure.Tests.IntegrationTests.RepositoryTests;

[Collection("Database_Seeding_Cleaning")]
public class Basic_Race_Repository_With_Default_Seed_Tests : IClassFixture<DbContextFixture>
{
    public DbContextFixture Fixture { get; }

    public Basic_Race_Repository_With_Default_Seed_Tests(DbContextFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Basic_List_No_Filter_Gets_All_100_Races()
    {
        // arrange
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var races = raceRepository.List(new ListRacesRequestFilter(), new ListRacesRequestOrder());

        // assert
        races.Should()
             .Contain(x => x.MeetingId == 1 || x.MeetingId == 2);

        races.Should()
             .HaveCount(100);
    }

    [Fact]
    public void List_With_Filter_Empty_Meeting_Ids_Gets_All_100_Races()
    {
        // arrange
        var raceRepository = new RaceRepository(Fixture.DbContext);

        // act
        var races = raceRepository.List(new ListRacesRequestFilter
                                        {
                                            MeetingIds =
                                            {
                                                Capacity = 0
                                            }
                                        }, new ListRacesRequestOrder());

        // assert
        races.Should()
             .Contain(x => x.MeetingId == 1 || x.MeetingId == 2);

        races.Should()
             .HaveCount(100);
    }
}