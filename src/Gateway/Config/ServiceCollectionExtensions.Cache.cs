namespace Gateway.Config;

public static class CacheExtensions
{
    public static void AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("Redis")!;

        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = connection;
        });
    }
}