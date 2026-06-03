var builder = WebApplication.CreateBuilder(args);

var githubPat = builder.Configuration["GITHUB_PAT"]
    ?? Environment.GetEnvironmentVariable("GITHUB_PAT")
    ?? throw new InvalidOperationException("GITHUB_PAT environment variable is required.");

var githubOrg = builder.Configuration["GITHUB_ORG"]
    ?? Environment.GetEnvironmentVariable("GITHUB_ORG")
    ?? throw new InvalidOperationException("GITHUB_ORG environment variable is required.");

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }));

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }
