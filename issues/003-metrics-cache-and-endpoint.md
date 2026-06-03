# Issue 3: In-memory metrics cache + API endpoint

## Parent PRD

See `prd/ai-usage-dashboard.md`

## What to build

A `MetricsCache` service that wraps `IMemoryCache` with a 1-hour TTL. On cache miss, it calls `GitHubCopilotClient` to refresh. Expose a `GET /api/metrics` minimal API endpoint that returns the cached metrics as JSON. This completes the backend data pipeline end-to-end.

## Acceptance criteria

- [ ] `MetricsCache` service registered in DI, wraps `IMemoryCache`
- [ ] Cache hit returns stored data without calling `GitHubCopilotClient`
- [ ] Cache miss triggers a call to `GitHubCopilotClient` and stores the result with 1h TTL
- [ ] `GET /api/metrics` returns cached metrics as JSON with appropriate content type
- [ ] `GET /api/metrics` returns 503 or similar if the GitHub API call fails
- [ ] Unit tests for `MetricsCache` (hit, miss, expiry)
- [ ] Integration test with `WebApplicationFactory` verifying `/api/metrics` response shape

## Blocked by

- Blocked by #2 (API client must exist)

## User stories addressed

- User story 9
- User story 10
