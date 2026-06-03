# Issue 1: Scaffold project structure + health endpoint

## Parent PRD

See `prd/ai-usage-dashboard.md`

## What to build

Set up the full project skeleton end-to-end: a .NET 9 minimal API project in `/api` that serves a Vue SPA from `wwwroot/`, a Vue 3 + Vite + TypeScript + Tailwind CSS project in `/client`, and a multi-stage Dockerfile that builds both and produces a single container. The app must validate that `GITHUB_PAT` and `GITHUB_ORG` environment variables are present at startup (fail fast if missing) and expose a `GET /api/health` endpoint returning 200 OK.

## Acceptance criteria

- [ ] .NET 9 minimal API project exists in `/api` with a solution file at the root
- [ ] Vue 3 + Vite + TypeScript + Tailwind CSS project exists in `/client`
- [ ] `GET /api/health` returns 200 with a simple JSON response
- [ ] App throws/exits on startup if `GITHUB_PAT` or `GITHUB_ORG` env vars are missing
- [ ] Multi-stage Dockerfile builds Vue, publishes .NET, and produces a working runtime image
- [ ] .NET app serves static files from `wwwroot/` in production
- [ ] Integration test using `WebApplicationFactory` verifies the health endpoint

## Blocked by

None - can start immediately

## User stories addressed

- User story 11
- User story 12
- User story 13
