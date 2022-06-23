using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Contracts;
using Microsoft.Data.Sqlite;
using Moq;
using Sports.Infrastructure.DataAccess;
using Sports.Services;
using Sports.UnitTests.Helpers;

namespace Sports.Tests.UnitTests;

public class RepositoryTests
{
    [Fact]
    public async Task Repo_List_No_Filter_Returns_4_Events()
    {
        // arrange
        var stubContext = new Mock<IDbContext>();
        stubContext.Setup(m => m.NewConnection())
                   .Returns(new SqliteConnection());
        var underTest = new Mock<SportsRepository>(args: stubContext.Object,
                                                   behavior: MockBehavior.Strict);

        underTest.Setup(m => m.List(It.IsAny<ListEventsRequestFilter>(),
                                    new ListEventsRequestOrder()))
                 .Returns(EventsList()
                             .AsReadOnly);

        var service = new SportsService(underTest.Object);

        // Act
        var response = await service.ListEvents(new ListEventsRequest
                                                {
                                                    Filter = new ListEventsRequestFilter(),
                                                    Order = new ListEventsRequestOrder()
                                                },
                                                TestServerCallContext.Create());


        // Assert
        underTest.Verify(x => x.List(new ListEventsRequestFilter(),
                                     new ListEventsRequestOrder()));

        response.Events.Should()
                .HaveCount(4);
    }

    [Fact]
    public async Task Repo_List_Sport_Filter_Cricket_Returns_2_Events()
    {
        // Arrange
        var stubContext = new Mock<IDbContext>();
        stubContext.Setup(m => m.NewConnection())
                   .Returns(new SqliteConnection());
        var underTest = new Mock<SportsRepository>(args: stubContext.Object,
                                                   behavior: MockBehavior.Loose);

        underTest.Setup(m => m.List(new ListEventsRequestFilter
                                    {
                                        Sport =
                                        {
                                            "Cricket"
                                        }
                                    },
                                    new ListEventsRequestOrder()))
                 .Returns(EventsList()
                         .Where(x => x.Sport == "Cricket")
                         .ToList()
                         .AsReadOnly);

        var service = new SportsService(underTest.Object);

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

        // assert
        underTest.Verify(x => x.List(new ListEventsRequestFilter
                                     {
                                         Sport =
                                         {
                                             "Cricket"
                                         }
                                     },
                                     new ListEventsRequestOrder()));

        response.Events.Should()
                .HaveCount(2);
    }

    [Fact]
    public async Task Repo_List_Visible_Only_Returns_1_Event()
    {
        // arrange
        var stubContext = new Mock<IDbContext>();
        stubContext.Setup(m => m.NewConnection())
                   .Returns(new SqliteConnection());
        var underTest = new Mock<SportsRepository>(args: stubContext.Object,
                                                   behavior: MockBehavior.Strict);

        underTest.Setup(m => m.List(It.IsAny<ListEventsRequestFilter>(),
                                    new ListEventsRequestOrder()))
                 .Returns(EventsList()
                         .Where(x => x.Visible)
                         .ToList()
                         .AsReadOnly);

        var service = new SportsService(underTest.Object);

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


        var exceptionCaught = false;
        // Assert

        // catch the MockException because this Verification will be false as 
        // we are passing OnlyVisibleEvents = true and the assertion is missing that property
        try
        {
            underTest.Verify(x => x.List(new ListEventsRequestFilter(),
                                         new ListEventsRequestOrder()));
        }
        catch (MockException e)
        {
            exceptionCaught = true;
        }

        Assert.True(exceptionCaught);
        underTest.Verify(x => x.List(new ListEventsRequestFilter
                                     {
                                         OnlyVisibleEvents = true
                                     },
                                     new ListEventsRequestOrder()));

        response.Events.Should()
                .HaveCount(1);
    }

    private static List<Event> EventsList()
    {
        return new List<Event>
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
                       Visible = false
                   },
                   new Event
                   {
                       Id = 2,
                       Sport = "Football",
                       Name = "Test2",
                       Visible = true,
                       AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2019,
                                                                                                      01,
                                                                                                      01),
                                                                                         DateTimeKind.Utc))
                   },
                   new Event
                   {
                       Id = 3,
                       Sport = "Soccer",
                       Name = "Test3",
                       Visible = false,
                       AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2018,
                                                                                                      01,
                                                                                                      01),
                                                                                         DateTimeKind.Utc))
                   },
                   new Event
                   {
                       Id = 4,
                       Sport = "Cricket",
                       Name = "Cricket",
                       Visible = false,
                       AdvertisedStartTime = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2017,
                                                                                                      01,
                                                                                                      01),
                                                                                         DateTimeKind.Utc))
                   }
               };
    }
}