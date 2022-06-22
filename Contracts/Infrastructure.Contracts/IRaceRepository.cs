using Racing;

namespace Infrastructure.Contracts;

public interface IRaceRepository
{
    /// <summary>
    ///     Retrieve a readonly collection of Races based on an input filter.
    /// </summary>
    /// <param name="filter">Instructs the repository on how to filter the races.</param>
    /// <returns>A read only collection of Race</returns>
    IReadOnlyCollection<Race> List(ListRacesRequestFilter filter);
}