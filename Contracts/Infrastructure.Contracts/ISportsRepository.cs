using Sports;

namespace Infrastructure.Contracts;

public interface ISportsRepository {
    /// <summary>
    ///     Retrieve a readonly collection of Events based on an input filter.
    /// </summary>
    /// <param name="filter">Instructs the repository on how to filter the events.</param>
    /// <param name="order">Instructs the repository on how to order the events.</param>
    /// <returns>A read only collection of Race.</returns>
    IReadOnlyCollection<Event> List(ListEventsRequestFilter filter, ListEventsRequestOrder order);

    /// <summary>
    ///     Retrieve a single Event by a given Id.
    /// </summary>
    /// <param name="id">The identifier of the Event.</param>
    /// <returns>The matching Event.</returns>
    Event Get(long id);
}