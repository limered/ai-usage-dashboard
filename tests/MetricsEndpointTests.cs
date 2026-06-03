using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiUsageDashboard.Tests;

public class MetricsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MetricsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("GITHUB_PAT", "test-token");
            builder.UseSetting("GITHUB_ORG", "test-org");
            builder.ConfigureServices(services =>
            {
                // Replace the HttpClient handler for GitHubCopilotClient with a fake
                services.AddHttpClient<AiUsageDashboard.Api.Services.GitHubCopilotClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => new FakeGitHubHandler());
            });
        });
    }

    [Fact]
    public async Task GetMetrics_Returns200WithJsonArray()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/metrics");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.True(doc.RootElement.GetArrayLength() > 0);
        var first = doc.RootElement[0];
        Assert.True(first.TryGetProperty("date", out _));
        Assert.True(first.TryGetProperty("totalActiveUsers", out _));
    }

    private class FakeGitHubHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var json = """
            [
              {
                "date": "2025-01-01",
                "total_suggestions_count": 100,
                "total_acceptances_count": 50,
                "total_lines_suggested": 200,
                "total_lines_accepted": 100,
                "total_active_users": 10,
                "breakdown": []
              }
            ]
            """;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}
