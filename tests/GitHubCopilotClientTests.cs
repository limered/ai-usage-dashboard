using System.Net;
using System.Text;
using AiUsageDashboard.Api.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AiUsageDashboard.Tests;

public class GitHubCopilotClientTests
{
    private const string SampleJson = """
        [
          {
            "date": "2024-01-15",
            "total_suggestions_count": 100,
            "total_acceptances_count": 50,
            "total_lines_suggested": 500,
            "total_lines_accepted": 250,
            "total_active_users": 10,
            "breakdown": [
              {
                "language": "csharp",
                "editor": "vscode",
                "suggestions_count": 60,
                "acceptances_count": 30,
                "lines_suggested": 300,
                "lines_accepted": 150,
                "active_users": 5
              }
            ]
          }
        ]
        """;

    private static GitHubCopilotClient CreateClient(HttpResponseMessage response)
    {
        var handler = new FakeHandler(response);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.github.com") };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["GITHUB_ORG"] = "test-org" })
            .Build();
        return new GitHubCopilotClient(httpClient, config);
    }

    [Fact]
    public async Task GetMetricsAsync_Success_DeserializesCorrectly()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(SampleJson, Encoding.UTF8, "application/json")
        };

        var client = CreateClient(response);
        var metrics = await client.GetMetricsAsync();

        Assert.Single(metrics);
        Assert.Equal("2024-01-15", metrics[0].Date);
        Assert.Equal(100, metrics[0].TotalSuggestionsCount);
        Assert.Equal(50, metrics[0].TotalAcceptancesCount);
        Assert.Equal(500, metrics[0].TotalLinesSuggested);
        Assert.Equal(250, metrics[0].TotalLinesAccepted);
        Assert.Equal(10, metrics[0].TotalActiveUsers);
        Assert.Single(metrics[0].Breakdown);
        Assert.Equal("csharp", metrics[0].Breakdown[0].Language);
        Assert.Equal("vscode", metrics[0].Breakdown[0].Editor);
        Assert.Equal(60, metrics[0].Breakdown[0].SuggestionsCount);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, "Invalid or missing GitHub PAT token.")]
    [InlineData(HttpStatusCode.Forbidden, "Token does not have permission to access Copilot metrics for this organization.")]
    [InlineData(HttpStatusCode.NotFound, "Organization not found or Copilot metrics not available.")]
    [InlineData((HttpStatusCode)429, "GitHub API rate limit exceeded. Retry later.")]
    public async Task GetMetricsAsync_ErrorStatus_ThrowsWithMessage(HttpStatusCode statusCode, string expectedMessage)
    {
        var response = new HttpResponseMessage(statusCode);
        var client = CreateClient(response);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetMetricsAsync());
        Assert.Equal(expectedMessage, ex.Message);
        Assert.Equal(statusCode, ex.StatusCode);
    }

    private class FakeHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(response);
    }
}
