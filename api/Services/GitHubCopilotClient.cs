using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AiUsageDashboard.Api.Models;

namespace AiUsageDashboard.Api.Services;

public class GitHubCopilotClient
{
    private readonly HttpClient _httpClient;
    private readonly string _org;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public GitHubCopilotClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _org = configuration["GITHUB_ORG"]
            ?? Environment.GetEnvironmentVariable("GITHUB_ORG")
            ?? throw new InvalidOperationException("GITHUB_ORG is required.");
    }

    public virtual async Task<List<DailyMetrics>> GetMetricsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/orgs/{_org}/copilot/metrics", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var message = response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Invalid or missing GitHub PAT token.",
                HttpStatusCode.Forbidden => "Token does not have permission to access Copilot metrics for this organization.",
                HttpStatusCode.NotFound => "Organization not found or Copilot metrics not available.",
                (HttpStatusCode)429 => "GitHub API rate limit exceeded. Retry later.",
                _ => $"GitHub API returned {(int)response.StatusCode}: {response.ReasonPhrase}"
            };
            throw new HttpRequestException(message, null, response.StatusCode);
        }

        var metrics = await response.Content.ReadFromJsonAsync<List<DailyMetrics>>(JsonOptions, cancellationToken);
        return metrics ?? [];
    }
}
