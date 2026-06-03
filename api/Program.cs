using AiUsageDashboard.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddScoped<MetricsCache>();

var githubPat = builder.Configuration["GITHUB_PAT"]
    ?? Environment.GetEnvironmentVariable("GITHUB_PAT")
    ?? throw new InvalidOperationException("GITHUB_PAT environment variable is required.");

var githubOrg = builder.Configuration["GITHUB_ORG"]
    ?? Environment.GetEnvironmentVariable("GITHUB_ORG")
    ?? throw new InvalidOperationException("GITHUB_ORG environment variable is required.");

builder.Services.AddHttpClient<GitHubCopilotClient>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {githubPat}");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
    client.DefaultRequestHeaders.Add("User-Agent", "ai-usage-dashboard");
});

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/api/metrics", async (MetricsCache cache, CancellationToken cancellationToken) =>
{
    try
    {
        var metrics = await cache.GetMetricsAsync(cancellationToken);
        return Results.Ok(metrics);
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 503);
    }
});

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }
