using System.Text;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Contracts;
using Microsoft.Data.Sqlite;
using Racing.Infrastructure.Queries;

namespace Racing.Infrastructure.DataAccess;

/// <inheritdoc cref="IRaceRepository" />
public class RaceRepository : IRaceRepository, IDisposable
{
    private readonly IDbContext _context;

    private readonly IEnumerable<string> _orderableFields = new[]
                                                            {
                                                                "id", "meeting_id", "advertised_start_time", "name", "number"
                                                            };

    public RaceRepository(IDbContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public virtual IReadOnlyCollection<Race> List(ListRacesRequestFilter filter, ListRacesRequestOrder order)
    {
        using var conn = _context.NewConnection();

        // apply filtering
        var (sb, sqlParams) = ApplyFilter(new(Query.For_SelectRaces()),
                                          filter);

        // apply ordering
        (sb, sqlParams) = ApplyOrder(sb,
                                     order,
                                     sqlParams);

        conn.Open();

        // sql command
        var cmd = conn.CreateCommand();
        cmd.CommandText = sb.ToString();
        cmd.Parameters.AddRange(sqlParams);

        // read from db
        var races = ScanRaces(cmd);

        return races.AsReadOnly();
    }

    public Race Get(long id)
    {
        using var conn = _context.NewConnection();

        // build simple query with 1 WHERE filter
        StringBuilder sb = new(Query.For_SelectRaces());
        sb.Append(" WHERE id = $id");

        // sql command
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sb.ToString();
        cmd.Parameters.Add(new SqliteParameter("$id",
                                               id));

        // leverage existing Scan method
        var race = ScanRaces(cmd)
           .FirstOrDefault();

        return race;
    }

    /// <summary>
    ///     Maps the filter object properties to the SQL equivalant and appends them to the StringBuilder as Where clauses
    /// </summary>
    /// <param name="query">The query as a StringBuilder object</param>
    /// <param name="filter">The filter request</param>
    /// <returns>The updated query and any parameters associated with the query</returns>
    private static (StringBuilder query, List<SqliteParameter> sqlParams) ApplyFilter(StringBuilder query, ListRacesRequestFilter filter)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (filter == null)
            return (query, new List<SqliteParameter>());

        var clauses = new List<string>();
        var sqlParams = new List<SqliteParameter>();

        // apply meeting ids filtering
        if (filter.MeetingIds.Any())
        {
            var paramNames = new List<string>();
            for (var i = 0; i < filter.MeetingIds.Count; i++)
            {
                // map the value and sql parameter name for query
                var pName = $"${i}";
                paramNames.Add(pName);
                sqlParams.Add(new SqliteParameter(pName,
                                                  filter.MeetingIds[i]));
            }

            // parameterised sql clause
            clauses.Add($"meeting_id IN ({string.Join(", ", paramNames)})");
        }

        // get races that are visible = true
        if (filter.OnlyVisibleRaces)
        {
            clauses.Add("visible = 1");
        }

        // append the filtering to the original query
        if (clauses.Count > 0)
        {
            query.Append($" WHERE {string.Join(" AND ", clauses)}");
        }

        return (query, sqlParams);
    }

    /// <summary>
    ///     Maps the order object properties to the SQL equivalant and appends them to the StringBuilder as ORDER BY clauses
    /// </summary>
    /// <param name="query">The query as a StringBuilder object</param>
    /// <param name="order">The order request</param>
    /// <param name="sqlParameters">The parameters already associated with the request</param>
    /// <returns>The updated query and any parameters associated with the query</returns>
    private (StringBuilder query, List<SqliteParameter> sqlParams) ApplyOrder(StringBuilder query, ListRacesRequestOrder order,
                                                                              List<SqliteParameter> sqlParameters)
    {
        if (string.IsNullOrWhiteSpace(order.Field))
            return (query, sqlParameters);

        if (!_orderableFields.Contains(order.Field))
            return (query, sqlParameters);

        // get the field we want from a safe list
        var field = _orderableFields.First(f => string.Equals(f,
                                                              order.Field,
                                                              StringComparison.InvariantCultureIgnoreCase));

        query.Append($" ORDER BY {field} ASC");

        return (query, sqlParameters);
    }

    /// <summary>
    ///     Opens the reader to the database and executes the query
    /// </summary>
    /// <param name="cmd">The SQL command to run</param>
    /// <returns>A list of matching Races</returns>
    protected virtual List<Race> ScanRaces(SqliteCommand cmd)
    {
        var races = new List<Race>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var id = reader.GetInt64(0);
            var meetingId = reader.GetInt64(1);
            var name = reader.GetString(2);
            var number = reader.GetInt64(3);
            var visible = reader.GetBoolean(4);
            var advertisedStart = reader.GetDateTime(5);
            advertisedStart = DateTime.SpecifyKind(advertisedStart,
                                                   DateTimeKind.Utc);

            var ts = Timestamp.FromDateTime(advertisedStart);

            var race = new Race
                       {
                           AdvertisedStartTime = ts,
                           Id = id,
                           Name = name,
                           Number = number,
                           Visible = visible,
                           MeetingId = meetingId,
                           Status = advertisedStart < DateTime.Now ? "CLOSED" : "OPEN"
                       };

            races.Add(race);
        }

        return races;
    }
}