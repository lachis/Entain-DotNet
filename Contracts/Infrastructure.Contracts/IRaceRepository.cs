using Racing;

namespace Infrastructure.Contracts;

public interface IRaceRepository
{
    /// <summary>
    ///     Retrieve a readonly collection of Races based on an input filter.
    /// </summary>
    /// <param name="filter">Instructs the repository on how to filter the races.</param>
    /// <param name="order">Instructs the repository on how to order the races.</param>
    /// <returns>A read only collection of Race.</returns>
    IReadOnlyCollection<Race> List(ListRacesRequestFilter filter, ListRacesRequestOrder order);

    /// <summary>
    ///     Retrieve a single Race by a given Id.
    /// </summary>
    /// <param name="id">The identifier of the Race.</param>
    /// <returns>The matching Race.</returns>
    Race Get(long id);
}