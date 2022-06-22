using Grpc.Core;
using Infrastructure.Contracts;

namespace Racing.Services;

public class RacingService : Racing.RacingBase
{
    private readonly IRaceRepository _raceRepository;

    public RacingService(IRaceRepository raceRepository)
    {
        _raceRepository = raceRepository;
    }

    // ListRaces will return a collection of races.
    public override  Task<ListRacesResponse> ListRaces(ListRacesRequest request, ServerCallContext context)
    {
        var response = _raceRepository.List(request.Filter);

        return Task.FromResult(new ListRacesResponse()
                               {
                                   Races =
                                   {
                                       response.ToArray()
                                   }
                               });
    }
}