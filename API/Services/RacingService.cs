using Grpc.Core;
using Racing;

namespace API.Services;

public class RacingService : Racing.Racing.RacingBase
{
    private readonly Racing.Racing.RacingClient _client;

    public RacingService(Racing.Racing.RacingClient client)
    {
        _client = client;
    }
   
    // HTTP Service wraps gRPC client which will forward the request to the gRPC Racing service (ie at http://localhost:7000)
    public override async Task<ListRacesResponse> ListRaces(ListRacesRequest request, ServerCallContext context)
    {
        return await _client.ListRacesAsync(request,
                               Metadata.Empty);
    }

    // HTTP Service wraps gRPC client which will forward the request to the gRPC Racing service (ie at http://localhost:7000)
    public override async Task<Race> GetRace(GetRaceRequest request, ServerCallContext context)
    {
        return await _client.GetRaceAsync(request,
                                            Metadata.Empty);
    }
}