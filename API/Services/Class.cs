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
    public override async Task<ListRacesResponse> ListRaces(ListRacesRequest request, ServerCallContext context)
    {
        return await _client.ListRacesAsync(request,
                               Metadata.Empty);
    }
}