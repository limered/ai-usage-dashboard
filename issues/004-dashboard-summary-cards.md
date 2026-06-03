# Issue 4: Dashboard summary cards

## Parent PRD

See `prd/ai-usage-dashboard.md`

## What to build

The Vue frontend's first real UI: fetch data from `GET /api/metrics` and display summary cards showing today's (or most recent day's) acceptance rate, active users count, total suggestions, and total acceptances. Style with Tailwind CSS.

## Acceptance criteria

- [ ] Vue app fetches `/api/metrics` on mount
- [ ] Displays acceptance rate as a percentage card
- [ ] Displays active users count card
- [ ] Displays total suggestions card
- [ ] Displays total acceptances card
- [ ] Cards use the most recent day's data from the response
- [ ] Handles loading and error states gracefully
- [ ] Styled with Tailwind CSS, responsive layout

## Blocked by

- Blocked by #3 (API endpoint must exist)

## User stories addressed

- User story 1
- User story 2
- User story 3
