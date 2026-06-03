using AiUsageDashboard.Api.Models;
using AiUsageDashboard.Api.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace AiUsageDashboard.Tests;

public class MetricsCacheTests
{
    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());
    private readonly Mock<GitHubCopilotClient> _clientMock;
    private readonly MetricsCache _sut;

    public MetricsCacheTests()
    {
        var httpClient = new HttpClient();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { "GITHUB_ORG", "test-org" } })
            .Build();
        _clientMock = new Mock<GitHubCopilotClient>(MockBehavior.Strict, httpClient, config);
        _sut = new MetricsCache(_memoryCache, _clientMock.Object);
    }

    [Fact]
    public async Task CacheMiss_CallsClient()
    {
        var expected = new List<DailyMetrics> { new() { Date = "2025-01-01", TotalActiveUsers = 5 } };
        _clientMock.Setup(c => c.GetMetricsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await _sut.GetMetricsAsync();

        Assert.Equal(expected, result);
        _clientMock.Verify(c => c.GetMetricsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CacheHit_DoesNotCallClientAgain()
    {
        var expected = new List<DailyMetrics> { new() { Date = "2025-01-01", TotalActiveUsers = 5 } };
        _clientMock.Setup(c => c.GetMetricsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        await _sut.GetMetricsAsync();
        var result = await _sut.GetMetricsAsync();

        Assert.Equal(expected, result);
        _clientMock.Verify(c => c.GetMetricsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
