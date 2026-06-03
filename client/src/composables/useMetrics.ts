import { ref } from 'vue'
import type { DailyMetric } from '../types'

export function useMetrics() {
  const data = ref<DailyMetric[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchMetrics() {
    loading.value = true
    error.value = null
    try {
      const res = await fetch('/api/metrics')
      if (!res.ok) throw new Error(`HTTP ${res.status}`)
      data.value = await res.json()
    } catch (e: any) {
      error.value = e.message ?? 'Failed to fetch metrics'
    } finally {
      loading.value = false
    }
  }

  fetchMetrics()

  return { data, loading, error, fetchMetrics }
}
