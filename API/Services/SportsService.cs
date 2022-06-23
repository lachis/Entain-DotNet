using Grpc.Core;
using Sports;

namespace API.Services;

public class SportsService : Sports.Sports.SportsBase
{
    private readonly Sports.Sports.SportsClient _client;

    public SportsService(Sports.Sports.SportsClient client)
    {
        _client = client;
    }

    // HTTP Service wraps gRPC client which will forward the request to the gRPC Racing service (ie at http://localhost:6000)
    public override async Task<ListEventsResponse> ListEvents(ListEventsRequest request, ServerCallContext context)
    {
        return await _client.ListEventsAsync(request,
                                             Metadata.Empty);
    }

    // HTTP Service wraps gRPC client which will forward the request to the gRPC Racing service (ie at http://localhost:6000)
    public override async Task<Event> GetEvent(GetEventRequest request, ServerCallContext context)
    {
        return await _client.GetEventAsync(request, Metadata.Empty);
    }
}