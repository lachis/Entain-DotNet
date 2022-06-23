using Google.Protobuf.WellKnownTypes;
using Infrastructure.Contracts;
using Moq;
using Racing.Services;
using Racing.UnitTests.Helpers;

namespace Racing.Tests.UnitTests;

public class RacingServiceTests
{
    [Fact]
    public async Task ListRaces_No_Filter_Returns_2_Races()
    {
        // Arrange
        var mockRepo = new Mock<IRaceRepository>();
        mockRepo.Setup(m => m.List(It.IsAny<ListRacesRequestFilter>()))
                .Returns(new List<Race>
                         {
                             new Race
                             {
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc)),
                                 Id = 1,
                                 MeetingId = 1,
                                 Name = "Test",
                                 Number = 1,
                                 Visible = true
                             },
                             new Race
                             {
                                 Id = 2,
                                 MeetingId = 2,
                                 Name = "Test2",
                                 Number = 2,
                                 Visible = true,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2020,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             }
                         }.AsReadOnly);
        var service = new RacingService(mockRepo.Object);

        // Act
        var response = await service.ListRaces(new ListRacesRequest
                                               {
                                                   Filter = new ListRacesRequestFilter()
                                               },
                                               TestServerCallContext.Create());

        // Assert
        Assert.True(response.Races.Count == 2);
        Assert.Equal(1,
                     response.Races[0]
                             .Id);
        Assert.Equal(2,
                     response.Races[1]
                             .Id);
    }

    [Fact]
    public async Task ListRaces_Filter_With_Meeting_Ids_Succeeds()
    {
        // Arrange
        var mockRepo = new Mock<IRaceRepository>();
        mockRepo.Setup(m => m.List(It.IsAny<ListRacesRequestFilter>()))
                .Returns(new List<Race>
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
                                 Visible = true
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
                                 MeetingId = 2,
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
                                 MeetingId = 2,
                                 Name = "Test4",
                                 Number = 4,
                                 Visible = false,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2017,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             }
                         }.AsReadOnly);
        var service = new RacingService(mockRepo.Object);

        // Act
        var response = await service.ListRaces(new ListRacesRequest
                                               {
                                                   Filter = new ListRacesRequestFilter
                                                            {
                                                                MeetingIds =
                                                                {
                                                                    2
                                                                }
                                                            }
                                               },
                                               TestServerCallContext.Create());

        // Assert
        Assert.True(response.Races.Count == 4);
        Assert.Equal(1,
                     response.Races[0]
                             .Id);
        Assert.Equal(2,
                     response.Races[1]
                             .Id);
        Assert.Equal(3,
                     response.Races[2]
                             .Id);
        Assert.Equal(4,
                     response.Races[3]
                             .Id);
    }

    [Fact]
    public async Task ListRaces_Filter_With_Visible_Races_Only_Succeeds()
    {
        // Arrange
        var mockRepo = new Mock<IRaceRepository>();
        mockRepo.Setup(m => m.List(It.IsAny<ListRacesRequestFilter>()))
                .Returns(new List<Race>
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
                                 Visible = true
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
                             }
                         }.AsReadOnly);
        var service = new RacingService(mockRepo.Object);

        // Act
        var response = await service.ListRaces(new ListRacesRequest
                                               {
                                                   Filter = new ListRacesRequestFilter
                                                            {
                                                                OnlyVisibleRaces = true
                                                            }
                                               },
                                               TestServerCallContext.Create());

        // Assert
        Assert.True(response.Races.Count == 2);
        Assert.Equal(1,
                     response.Races[0]
                             .Id);
        Assert.Equal(2,
                     response.Races[1]
                             .Id);
    }
}