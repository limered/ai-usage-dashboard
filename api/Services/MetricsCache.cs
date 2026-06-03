using AiUsageDashboard.Api.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AiUsageDashboard.Api.Services;

public class MetricsCache
{
    private const string CacheKey = "copilot_metrics";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    private readonly IMemoryCache _cache;
    private readonly GitHubCopilotClient _client;

    public MetricsCache(IMemoryCache cache, GitHubCopilotClient client)
    {
        _cache = cache;
        _client = client;
    }

    public async Task<List<DailyMetrics>> GetMetricsAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out List<DailyMetrics>? cached) && cached is not null)
        {
            return cached;
        }

        var metrics = await _client.GetMetricsAsync(cancellationToken);
        _cache.Set(CacheKey, metrics, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        });
        return metrics;
    }
}
