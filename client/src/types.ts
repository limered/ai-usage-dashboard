export interface MetricBreakdown {
  language: string
  editor: string
  suggestions_count: number
  acceptances_count: number
  lines_suggested: number
  lines_accepted: number
  active_users: number
}

export interface DailyMetric {
  date: string
  total_suggestions_count: number
  total_acceptances_count: number
  total_lines_suggested: number
  total_lines_accepted: number
  total_active_users: number
  breakdown: MetricBreakdown[]
}

export type MetricsResponse = DailyMetric[]
