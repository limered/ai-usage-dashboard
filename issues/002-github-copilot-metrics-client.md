# Issue 2: GitHub Copilot Metrics API client

## Parent PRD

See `prd/ai-usage-dashboard.md`

## What to build

A `GitHubCopilotClient` service that calls GitHub's Copilot Metrics API (`GET /orgs/{org}/copilot/metrics`) using the configured PAT. It should deserialize the response into strongly-typed C# models covering daily metrics with breakdowns by language, editor, and model. It must handle error cases gracefully (rate limiting, auth failures, network errors).

## Acceptance criteria

- [ ] `GitHubCopilotClient` class registered in DI, accepts `HttpClient` via `IHttpClientFactory`
- [ ] Calls `GET /orgs/{org}/copilot/metrics` with `Authorization: Bearer {token}` header
- [ ] Deserializes response into typed models (daily entries with suggestions, acceptances, active users, language/editor breakdowns)
- [ ] Handles 401, 403, 404, 429, and network failure cases with meaningful exceptions or error results
- [ ] Unit tests with mocked `HttpMessageHandler` covering success path and all error cases

## Blocked by

- Blocked by #1 (project structure must exist)

## User stories addressed

- User story 9
