using Grpc.Core;

namespace Racing.Services;

public class RacingService : Racing.RacingBase
{
    private readonly ILogger _logger;

    public RacingService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RacingService>();
    }

    public override async Task<ListRacesResponse> ListRaces(ListRacesRequest request, ServerCallContext context)
    {
        //     _logger.LogInformation($"Sending hello to {request.Name}");
        //return Task.FromResult(new ListRacesResponse() /*{ Message = "Hello " + request.Name })*/;
        return new ListRacesResponse();
    }
}