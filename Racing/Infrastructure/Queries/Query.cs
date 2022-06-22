namespace Racing.Infrastructure.Queries;

public static class Query
{
    public static string For_SelectRaces()
    {
        return @"
  SELECT 
				id, 
				meeting_id, 
				name, 
				number, 
				visible, 
				advertised_start_time 
			FROM races";
    }
}