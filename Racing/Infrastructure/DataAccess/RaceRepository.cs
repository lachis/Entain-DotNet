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

    public RaceRepository(IDbContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public IReadOnlyCollection<Race> List(ListRacesRequestFilter filter)
    {
        using var conn = _context.NewConnection();

        var (sb, sqlParams) = ApplyFilter(new(Query.For_SelectRaces()),
                                          filter);

        conn.Open();
        var cmd = conn.CreateCommand();

        cmd.CommandText = sb.ToString();
        cmd.Parameters.AddRange(sqlParams);

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
                           MeetingId = meetingId
                       };

            races.Add(race);
        }


        return races.AsReadOnly();
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

        if (filter.MeetingIds.Any())
        {
            var paramNames = new List<string>();
            for (var i = 0; i < filter.MeetingIds.Count; i++)
            {
                var pName = $"${i}";
                paramNames.Add(pName);
                sqlParams.Add(new SqliteParameter(pName,
                                                  filter.MeetingIds[i]));
            }

            clauses.Add($"meeting_id IN ({string.Join(", ", paramNames)})");
        }

        // get races that are visible = true
        if (filter.OnlyVisibleRaces)
        {
            clauses.Add("visible = 1");
        }

        if (clauses.Count > 0)
        {
            query.Append($" WHERE {string.Join(" AND ", clauses)}");
        }

        return (query, sqlParams);
    }
}