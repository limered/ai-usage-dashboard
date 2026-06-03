# AI Usage Dashboard PRD

## Problem Statement

Our team uses GitHub Copilot but we have no visibility into how effectively it's being used across the organization. GitHub's built-in UI provides some metrics, but there's no easy way to view trends over time, compare language/editor adoption, or surface key metrics at a glance without navigating GitHub's admin console. We need a lightweight, always-accessible dashboard that shows Copilot usage at the org level.

## Solution

A single-page web dashboard that pulls data from GitHub's Copilot Metrics API, caches it in memory, and presents it as summary cards and trend charts. The dashboard shows the last 28 days of data (the API's rolling window), requires no authentication for viewers, and is deployed as a single Docker container on Render.

## User Stories

1. As a team lead, I want to see the current Copilot acceptance rate at a glance, so that I can understand how useful Copilot is to the team.
2. As a team lead, I want to see the number of active Copilot users today, so that I know how many people are actually using it.
3. As a team lead, I want to see total suggestions and acceptances as summary cards, so that I can quickly assess overall volume.
4. As a team lead, I want to see a trend chart of acceptance rate over 28 days, so that I can spot improvements or regressions in Copilot effectiveness.
5. As a team lead, I want to see a trend chart of active users over 28 days, so that I can track adoption growth.
6. As a team lead, I want to see total suggestions/acceptances trended over 28 days, so that I can see usage volume changes.
7. As a team lead, I want to see a breakdown of Copilot usage by programming language, so that I know which languages benefit most from Copilot.
8. As a team lead, I want to see a breakdown of Copilot usage by editor/IDE, so that I understand which development environments my team prefers with Copilot.
9. As a team lead, I want the dashboard to load quickly without hitting GitHub's API on every request, so that the experience is responsive.
10. As a team lead, I want the dashboard to automatically refresh its data periodically, so that I always see reasonably current metrics without manual intervention.
11. As a developer, I want the app to fail fast on startup if the GitHub PAT or org name is not configured, so that misconfiguration is immediately obvious.
12. As a developer, I want a health endpoint, so that Render can monitor the service and I can verify the app is running.
13. As an operator, I want the app deployed as a single Docker container, so that deployment and scaling is simple.

## Implementation Decisions

- **Architecture:** Single .NET 9 solution with a `/api` folder for the backend and `/client` folder for the Vue SPA. Built and deployed as one unit.
- **API style:** ASP.NET Core Minimal APIs. Two endpoints: `GET /api/metrics` (returns cached Copilot metrics as JSON) and `GET /api/health`.
- **GitHub API integration:** A `GitHubCopilotClient` service that calls `GET /orgs/{org}/copilot/metrics` using a PAT. Authenticates via `Authorization: Bearer {token}` header. Deserializes the response into strongly-typed C# models.
- **Caching:** A `MetricsCache` service wrapping `IMemoryCache` with a 1-hour TTL. On cache miss, it calls `GitHubCopilotClient` to refresh. The cache is lost on restart, which is acceptable since the API always has the last 28 days.
- **Configuration:** Two environment variables only: `GITHUB_PAT` and `GITHUB_ORG`. Read at startup, validated immediately — app throws if either is missing.
- **Frontend:** Vue 3 + Vite + TypeScript + Tailwind CSS. Single view (no Vue Router). Summary cards at the top, trend line charts in the middle, language/editor bar charts at the bottom.
- **Charting:** Chart.js via `vue-chartjs` for all visualizations.
- **Static file serving:** In production, the .NET app serves the built Vue app from `wwwroot/`. In development, Vue dev server runs on a separate port with a proxy to the .NET API.
- **Dockerfile:** Multi-stage build — Stage 1: Node image builds the Vue app. Stage 2: .NET SDK publishes the API. Stage 3: .NET runtime image copies both outputs and serves.
- **Render deployment:** Single Web Service, Docker environment, environment variables set in Render dashboard.

## Testing Decisions

- **Testing philosophy:** Test external behavior through public interfaces, not implementation details. Mock external dependencies (GitHub API) at the HTTP boundary. Tests should be resilient to refactoring.
- **GitHubCopilotClient tests:** Unit tests with a mocked `HttpMessageHandler`. Verify correct URL construction, header usage, deserialization of API responses, and error handling (rate limits, 404s, network failures).
- **MetricsCache tests:** Unit tests verifying cache-hit returns stored data without calling the client, cache-miss triggers a refresh, and TTL expiry causes re-fetch. Use a fake time provider or short TTLs.
- **API integration tests:** Use `WebApplicationFactory` to spin up the app in-memory. Mock the GitHub API at the `HttpClient` level. Verify endpoint responses, status codes, and JSON shape.
- **No frontend tests in v1** — the Vue layer is thin presentation logic.

## Out of Scope

- Per-user usage metrics (not available via API)
- Team-level breakdowns (org has no teams)
- Seat utilization / billing information
- Data persistence / database (no historical data beyond 28 days)
- User authentication on the dashboard
- Alerting or notifications
- GitHub App authentication (using PAT instead)
- Multiple org support

## Further Notes

- The GitHub Copilot Metrics API only retains 28 days of rolling data. If persistence is added later, a daily background job should be introduced to store snapshots before they roll off.
- .NET 9 is STS with support ending May 2026. Plan to migrate to .NET 10 (LTS) when it releases in November 2025, or accept the short support window.
- The Copilot Metrics API is relatively new and may change. The `GitHubCopilotClient` module isolates this risk — if the API changes, only that module needs updating.
