# Issue 5: Trend charts (28-day line charts)

## Parent PRD

See `prd/ai-usage-dashboard.md`

## What to build

Add Chart.js line charts to the dashboard showing 28-day trends for: acceptance rate, active users, and total suggestions/acceptances. Use `vue-chartjs` for Vue integration.

## Acceptance criteria

- [ ] Line chart showing acceptance rate over 28 days
- [ ] Line chart showing active users over 28 days
- [ ] Line chart showing suggestions and acceptances over 28 days (dual series)
- [ ] Charts use data already fetched from `/api/metrics` (no additional API calls)
- [ ] X-axis shows dates, Y-axis shows appropriate values
- [ ] Charts are responsive and readable on different screen sizes

## Blocked by

- Blocked by #4 (dashboard structure and data fetching must exist)

## User stories addressed

- User story 4
- User story 5
- User story 6
