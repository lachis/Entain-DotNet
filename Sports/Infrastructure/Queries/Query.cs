namespace Sports.Infrastructure.Queries;

public static class Query
{
    public static string For_SelectEvents()
    {
        return @"
  SELECT 
				id, 
				name, 
				sport, 
				visible, 
				advertised_start_time 
			FROM events";
    }
}