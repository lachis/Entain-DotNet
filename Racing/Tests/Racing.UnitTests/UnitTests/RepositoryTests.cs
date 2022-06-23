using Google.Protobuf.WellKnownTypes;
using Infrastructure.Contracts;
using Microsoft.Data.Sqlite;
using Moq;
using Racing.Infrastructure.DataAccess;
using Racing.Services;
using Racing.UnitTests.Helpers;

namespace Racing.Tests.UnitTests;

public class RepositoryTests
{
    [Fact]
    public async Task Repo_List_No_Filter_Returns_4_Races()
    {
        // arrange
        var stubContext = new Mock<IDbContext>();
        stubContext.Setup(m => m.NewConnection())
                   .Returns(new SqliteConnection());
        var underTest = new Mock<RaceRepository>(args: stubContext.Object,
                                                 behavior: MockBehavior.Strict);

        underTest.Setup(m => m.List(It.IsAny<ListRacesRequestFilter>(), new ListRacesRequestOrder()))
                 .Returns(RaceList()
                             .AsReadOnly);

        var service = new RacingService(underTest.Object);

        // Act
        var response = await service.ListRaces(new ListRacesRequest
                                               {
                                                   Filter = new ListRacesRequestFilter(),
                                                   Order = new ListRacesRequestOrder()
                                               },
                                               TestServerCallContext.Create());


        // Assert
        underTest.Verify(x => x.List(new ListRacesRequestFilter(), new ListRacesRequestOrder()));

        Assert.True(response.Races.Count == 4);
    }

    [Fact]
    public async Task Repo_List_MeetingId_2_Returns_2_Races()
    {
        // Arrange
        var stubContext = new Mock<IDbContext>();
        stubContext.Setup(m => m.NewConnection())
                   .Returns(new SqliteConnection());
        var underTest = new Mock<RaceRepository>(args: stubContext.Object,
                                                 behavior: MockBehavior.Loose);

        underTest.Setup(m => m.List(new ListRacesRequestFilter
                                    {
                                        MeetingIds =
                                        {
                                            2
                                        }
                                    }, new ListRacesRequestOrder()))
                 .Returns(RaceList()
                         .Where(x => x.MeetingId == 2)
                         .ToList()
                         .AsReadOnly);

        var service = new RacingService(underTest.Object);

        // Act
        var response = await service.ListRaces(new ListRacesRequest
                                               {
                                                   Filter = new ListRacesRequestFilter
                                                            {
                                                                MeetingIds =
                                                                {
                                                                    2
                                                                }
                                                            },
                                                   Order = new ListRacesRequestOrder()
                                               },
                                               TestServerCallContext.Create());

        // assert
        underTest.Verify(x => x.List(new ListRacesRequestFilter
                                     {
                                         MeetingIds =
                                         {
                                             2
                                         }
                                     }, new ListRacesRequestOrder()));

        Assert.True(response.Races.Count == 2);
    }

    [Fact]
    public async Task Repo_List_Visible_Only_Returns_1_Race()
    {
        // arrange
        var stubContext = new Mock<IDbContext>();
        stubContext.Setup(m => m.NewConnection())
                   .Returns(new SqliteConnection());
        var underTest = new Mock<RaceRepository>(args: stubContext.Object,
                                                 behavior: MockBehavior.Strict);

        underTest.Setup(m => m.List(It.IsAny<ListRacesRequestFilter>(), new ListRacesRequestOrder()))
                 .Returns(RaceList()
                         .Where(x => x.Visible)
                         .ToList()
                         .AsReadOnly);

        var service = new RacingService(underTest.Object);

        // Act
        var response = await service.ListRaces(new ListRacesRequest
                                               {
                                                   Filter = new ListRacesRequestFilter
                                                            {
                                                                OnlyVisibleRaces = true
                                                            },
                                                   Order = new ListRacesRequestOrder()
                                               },
                                               TestServerCallContext.Create());


        var exceptionCaught = false;
        // Assert

        // catch the MockException because this Verification will be false as 
        // we are passing OnlyVisibleRaces = true and the assertion is missing that property
        try
        {
            underTest.Verify(x => x.List(new ListRacesRequestFilter(), new ListRacesRequestOrder()));
        }
        catch (MockException e)
        {
            exceptionCaught = true;
        }

        Assert.True(exceptionCaught);
        underTest.Verify(x => x.List(new ListRacesRequestFilter
                                     {
                                         OnlyVisibleRaces = true
                                     }, new ListRacesRequestOrder()));

        Assert.True(1 == response.Races.Count);
    }

    private static List<Race> RaceList()
    {
        return new List<Race>
               {
                   new Race
                   {
                       AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021,
                                                                                                      01,
                                                                                                      01),
                                                                                         DateTimeKind.Utc)),
                       Id = 1,
                       MeetingId = 2,
                       Name = "Test",
                       Number = 1,
                       Visible = false
                   },
                   new Race
                   {
                       Id = 2,
                       MeetingId = 2,
                       Name = "Test2",
                       Number = 2,
                       Visible = true,
                       AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2019,
                                                                                                      01,
                                                                                                      01),
                                                                                         DateTimeKind.Utc))
                   },
                   new Race
                   {
                       Id = 3,
                       MeetingId = 3,
                       Name = "Test3",
                       Number = 3,
                       Visible = false,
                       AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2018,
                                                                                                      01,
                                                                                                      01),
                                                                                         DateTimeKind.Utc))
                   },
                   new Race
                   {
                       Id = 4,
                       MeetingId = 4,
                       Name = "Test4",
                       Number = 4,
                       Visible = false,
                       AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2017,
                                                                                                      01,
                                                                                                      01),
                                                                                         DateTimeKind.Utc))
                   }
               };
    }
}