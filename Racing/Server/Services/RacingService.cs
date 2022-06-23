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

    /// <summary>
    ///  ListRaces will return a collection of races.
    /// </summary>
    /// <param name="request">The gRPC request object</param>
    /// <param name="context">The gRPC request context</param>
    /// <returns>The gRPC response object containing the race resources</returns>
    public override Task<ListRacesResponse> ListRaces(ListRacesRequest request, ServerCallContext context)
    {
        var response = _raceRepository.List(request.Filter, request.Order);

        return Task.FromResult(new ListRacesResponse()
                               {
                                   Races =
                                   {
                                       response.ToArray()
                                   }
                               });
    }

    /// <summary>
    ///  GetRace will return a single race.
    /// </summary>
    /// <param name="request">The gRPC request object</param>
    /// <param name="context">The gRPC request context</param>
    /// <returns>The race resource</returns>
    public override Task<Race> GetRace(GetRaceRequest request, ServerCallContext context)
    {
        var race = _raceRepository.Get(request.Id);

        return Task.FromResult(race);
    }
}