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
        var mockRepo = new Mock<RaceRepository>(args: stubContext.Object,
                                                behavior: MockBehavior.Strict);
     
        mockRepo.Setup(m => m.List(It.IsAny<ListRacesRequestFilter>()))
                .Returns(RaceList()
                            .AsReadOnly);

        var service = new RacingService(mockRepo.Object);

        // Act
        var response = await service.ListRaces(new ListRacesRequest()
                                               {
                                                   Filter = new ListRacesRequestFilter()
                                               },
                                               TestServerCallContext.Create());


        // Assert
        mockRepo.Verify(x=>x.List(new ListRacesRequestFilter()));

        Assert.Equal(4,
                     response.Races.Count);
    }

    [Fact]
    public async Task Repo_List_MeetingId_2_Returns_2_Races()
    {
        var stubContext = new Mock<IDbContext>();
        stubContext.Setup(m => m.NewConnection())
                   .Returns(new SqliteConnection());
        var mockRepo = new Mock<RaceRepository>(args: stubContext.Object,
                                                behavior: MockBehavior.Loose);

        mockRepo.Setup(m => m.List(new ListRacesRequestFilter()
                                   {
                                       MeetingIds =
                                       {
                                           2
                                       }
                                   }))
                .Returns(RaceList()
                        .Where(x => x.MeetingId == 2)
                        .ToList()
                        .AsReadOnly);

        var service = new RacingService(mockRepo.Object);

        // Act
        var response = await service.ListRaces(new ListRacesRequest()
                                               {
                                                   Filter = new ListRacesRequestFilter()
                                                            {
                                                                MeetingIds =
                                                                {
                                                                    2
                                                                }
                                                            }
                                               },
                                               TestServerCallContext.Create());

        // assert
        mockRepo.Verify(x => x.List(new ListRacesRequestFilter()
                                    {
                                        MeetingIds =
                                        {
                                            2
                                        }
                                    }));

        Assert.Equal(2,
                     response.Races.Count);
    }

    [Fact]
    public async Task Repo_List_Visible_Only_Returns_1_Race()
    {
        // arrange
        var stubContext = new Mock<IDbContext>();
        stubContext.Setup(m => m.NewConnection())
                   .Returns(new SqliteConnection());
        var mockRepo = new Mock<RaceRepository>(args: stubContext.Object,
                                                behavior: MockBehavior.Strict);

        mockRepo.Setup(m => m.List(It.IsAny<ListRacesRequestFilter>()))
                .Returns(RaceList()
                        .Where(x => x.Visible)
                        .ToList()
                        .AsReadOnly);

        var service = new RacingService(mockRepo.Object);

        // Act
        var response = await service.ListRaces(new ListRacesRequest()
                                               {
                                                   Filter = new ListRacesRequestFilter()
                                                            {
                                                                OnlyVisibleRaces = true
                                                            }
                                               },
                                               TestServerCallContext.Create());


        bool exceptionCaught = false;
        // Assert
        
        // catch the MockException because this Verification will be false as 
        // we are passing OnlyVisibleRaces = true and the assertion is missing that property
        try
        {
            mockRepo.Verify(x => x.List(new ListRacesRequestFilter()));
        }
        catch (Moq.MockException e)
        {
            exceptionCaught = true;
        }

        Assert.True(exceptionCaught);
        mockRepo.Verify(x => x.List(
                                    new ListRacesRequestFilter()
                                    {
                                        OnlyVisibleRaces = true
                                    }));

        Assert.Equal(1,
                     response.Races.Count);
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