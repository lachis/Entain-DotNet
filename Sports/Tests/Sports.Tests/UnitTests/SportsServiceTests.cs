using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Contracts;
using Moq;
using Sports.Services;
using Sports.UnitTests.Helpers;

namespace Sports.Tests.UnitTests;

public class SportsServiceTests
{
    [Fact]
    public async Task ListEvents_No_Filter_Returns_2_Events()
    {
        // Arrange
        var mockRepo = new Mock<ISportsRepository>();
        mockRepo.Setup(m => m.List(It.IsAny<ListEventsRequestFilter>(),
                                   new ListEventsRequestOrder()))
                .Returns(new List<Event>
                         {
                             new Event
                             {
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc)),
                                 Id = 1,
                                 Sport = "Cricket",
                                 Name = "Test",
                                 Visible = true
                             },
                             new Event
                             {
                                 Id = 2,
                                 Name = "Test2",
                                 Sport = "Football",
                                 Visible = true,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2020,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             }
                         }.AsReadOnly);
        var service = new SportsService(mockRepo.Object);

        // Act
        var response = await service.ListEvents(new ListEventsRequest
                                                {
                                                    Filter = new ListEventsRequestFilter(),
                                                    Order = new ListEventsRequestOrder()
                                                },
                                                TestServerCallContext.Create());

        // Assert
        response.Events.Should()
                .HaveCount(2);

        response.Events.Should()
                .Contain(x => x.Id == 1);

        response.Events.Should()
                .Contain(x => x.Id == 2);
    }

    [Fact]
    public async Task ListEvents_Filter_With_Sports_Succeeds()
    {
        // Arrange
        var mockRepo = new Mock<ISportsRepository>();
        mockRepo.Setup(m => m.List(It.IsAny<ListEventsRequestFilter>(),
                                   new ListEventsRequestOrder()))
                .Returns(new List<Event>
                         {
                             new Event
                             {
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc)),
                                 Id = 1,
                                 Sport = "Cricket",
                                 Name = "Test",
                                 Visible = true
                             },
                             new Event
                             {
                                 Id = 2,
                                 Name = "Test2",
                                 Sport = "Football",
                                 Visible = true,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2020,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             },
                             new Event
                             {
                                 Id = 4,
                                 Sport = "Cricket",
                                 Name = "Test3",
                                 Visible = false,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2018,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             },
                             new Event
                             {
                                 Id = 5,
                                 Sport = "Tennis",
                                 Name = "Test4",
                                 Visible = false,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2017,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             }
                         }.Where(x => x.Sport == "Cricket")
                          .ToList()
                          .AsReadOnly);
        var service = new SportsService(mockRepo.Object);

        // Act
        var response = await service.ListEvents(new ListEventsRequest
                                                {
                                                    Filter = new ListEventsRequestFilter
                                                             {
                                                                 Sport =
                                                                 {
                                                                     "Cricket"
                                                                 }
                                                             },
                                                    Order = new ListEventsRequestOrder()
                                                },
                                                TestServerCallContext.Create());

        // Assert
        response.Events.Should()
                .HaveCount(2);

        response.Events.Should()
                .Contain(x => x.Id == 1);

        response.Events.Should()
                .Contain(x => x.Id == 4);
    }

    [Fact]
    public async Task ListEvents_Filter_With_Visible_Events_Only_Succeeds()
    {
        // Arrange
        var mockRepo = new Mock<ISportsRepository>();
        mockRepo.Setup(m => m.List(It.IsAny<ListEventsRequestFilter>(),
                                   new ListEventsRequestOrder()))
                .Returns(new List<Event>
                         {
                             new Event
                             {
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc)),
                                 Id = 1,
                                 Sport = "Cricket",
                                 Name = "Test",
                                 Visible = true
                             },
                             new Event
                             {
                                 Id = 2,
                                 Name = "Test2",
                                 Sport = "Football",
                                 Visible = true,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2020,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             },
                             new Event
                             {
                                 Id = 4,
                                 Sport = "Cricket",
                                 Name = "Test3",
                                 Visible = false,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2018,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             },
                             new Event
                             {
                                 Id = 5,
                                 Sport = "Tennis",
                                 Name = "Test4",
                                 Visible = false,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2017,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             }
                         }.Where(x => x.Visible)
                          .ToList()
                          .AsReadOnly);
        var service = new SportsService(mockRepo.Object);

        // Act
        var response = await service.ListEvents(new ListEventsRequest
                                                {
                                                    Filter = new ListEventsRequestFilter
                                                             {
                                                                 OnlyVisibleEvents = true
                                                             },
                                                    Order = new ListEventsRequestOrder()
                                                },
                                                TestServerCallContext.Create());

        // Assert
        response.Events.Should()
                .HaveCount(2);

        response.Events.Should()
                .Contain(x => x.Id == 1);

        response.Events.Should()
                .Contain(x => x.Id == 2);
    }

    [Fact]
    public async Task ListEvents_Filter_With_Visible_OrderBy_Start_Time_Returns_Visible_Only_OrderedBy_Start_Time()
    {
        // Arrange
        var mockRepo = new Mock<ISportsRepository>();
        mockRepo.Setup(m => m.List(It.IsAny<ListEventsRequestFilter>(),
                                   It.IsAny<ListEventsRequestOrder>()))
                .Returns(new List<Event>
                         {
                             new Event
                             {
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc)),
                                 Id = 1,
                                 Sport = "Cricket",
                                 Name = "Test",
                                 Visible = true
                             },
                             new Event
                             {
                                 Id = 2,
                                 Name = "Test2",
                                 Sport = "Football",
                                 Visible = true,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2020,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             },
                             new Event
                             {
                                 Id = 4,
                                 Sport = "Cricket",
                                 Name = "Test3",
                                 Visible = false,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2018,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             },
                             new Event
                             {
                                 Id = 5,
                                 Sport = "Tennis",
                                 Name = "Test4",
                                 Visible = true,
                                 AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2017,
                                                                                                                01,
                                                                                                                01),
                                                                                                   DateTimeKind.Utc))
                             }
                         }.Where(x=>x.Visible).OrderBy(x => x.AdvertisedStartTime)
                          .ToList()
                          .AsReadOnly);
        var service = new SportsService(mockRepo.Object);

        // Act
        var response = await service.ListEvents(new ListEventsRequest
                                                {
                                                    Filter = new ListEventsRequestFilter
                                                             {
                                                                 OnlyVisibleEvents = true
                                                             },
                                                    Order = new ListEventsRequestOrder
                                                            {
                                                                Field = "advertised_start_time"
                                                            }
                                                },
                                                TestServerCallContext.Create());

        // Assert
        var responseEvents = response.Events;
        responseEvents.Should()
                      .BeInAscendingOrder(x => x.AdvertisedStartTime);

        responseEvents.Should()
                      .HaveCount(3);

        responseEvents.Should()
                      .OnlyContain(x => x.Visible);
    }

    [Fact]
    public async Task GetEvent_With_Id_1_Returns_Event_Id_1()
    {
        // Arrange
        var mockRepo = new Mock<ISportsRepository>();
        mockRepo.Setup(m => m.Get(It.IsAny<long>()))
                .Returns(new Event
                         {
                             AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021,
                                                                                                            01,
                                                                                                            01),
                                                                                               DateTimeKind.Utc)),
                             Id = 1,
                             Sport = "Cricket",
                             Name = "Test",
                             Visible = true
                         });
        var service = new SportsService(mockRepo.Object);

        // Act
        var response = await service.GetEvent(new GetEventRequest
                                              {
                                                  Id = 1
                                              },
                                              TestServerCallContext.Create());

        // Assert
        response.Should()
                .Match<Event>(x => x.Id == 1);
    }

    [Fact]
    public async Task GetEvent_With_Id_3001_Returns_Null()
    {
        // Arrange
        var mockRepo = new Mock<ISportsRepository>();
        mockRepo.Setup(m => m.Get(It.IsAny<long>()))
                .Returns(() => null);
        var service = new SportsService(mockRepo.Object);

        // Act
        var response = await service.GetEvent(new GetEventRequest
                                              {
                                                  Id = 3001
                                              },
                                              TestServerCallContext.Create());

        // Assert
        response.Should()
                .BeNull();
    }
}