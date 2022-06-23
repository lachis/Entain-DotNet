using Grpc.Core;
using Infrastructure.Contracts;

namespace Sports.Services;

public class SportsService : Sports.SportsBase
{
    private readonly ISportsRepository _sportsRepository;

    public SportsService(ISportsRepository sportsRepository)
    {
        _sportsRepository = sportsRepository;
    }

    /// <summary>
    ///  ListEvents will return a collection of events.
    /// </summary>
    /// <param name="request">The gRPC request object</param>
    /// <param name="context">The gRPC request context</param>
    /// <returns>The gRPC response object containing the event resources</returns>
    public override Task<ListEventsResponse> ListEvents(ListEventsRequest request, ServerCallContext context)
    {
        var response = _sportsRepository.List(request.Filter, request.Order);

        return Task.FromResult(new ListEventsResponse()
                               {
                                   Events =
                                   {
                                       response.ToArray()
                                   }
                               });
    }

    /// <summary>
    ///  GetEvent will return a single event.
    /// </summary>
    /// <param name="request">The gRPC request object</param>
    /// <param name="context">The gRPC request context</param>
    /// <returns>The event resource</returns>
    public override Task<Event> GetEvent(GetEventRequest request, ServerCallContext context)
    {
        var race = _sportsRepository.Get(request.Id);

        return Task.FromResult(race);
    }
}