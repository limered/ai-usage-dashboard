<script setup lang="ts">
import { computed } from 'vue'
import { Bar } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js'
import type { DailyMetric } from '../types'

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

const props = defineProps<{ metrics: DailyMetric[] }>()

interface AggEntry {
  suggestions: number
  acceptances: number
}

function aggregate(key: 'language' | 'editor') {
  const map = new Map<string, AggEntry>()
  for (const day of props.metrics) {
    for (const b of day.breakdown) {
      const label = b[key] || 'Unknown'
      const entry = map.get(label) ?? { suggestions: 0, acceptances: 0 }
      entry.suggestions += b.suggestions_count
      entry.acceptances += b.acceptances_count
      map.set(label, entry)
    }
  }
  return [...map.entries()]
    .sort((a, b) => b[1].suggestions - a[1].suggestions)
    .slice(0, 10)
}

const languageData = computed(() => {
  const entries = aggregate('language')
  return {
    labels: entries.map(([l]) => l),
    datasets: [
      {
        label: 'Suggestions',
        data: entries.map(([, v]) => v.suggestions),
        backgroundColor: 'rgba(59, 130, 246, 0.7)',
      },
      {
        label: 'Acceptances',
        data: entries.map(([, v]) => v.acceptances),
        backgroundColor: 'rgba(16, 185, 129, 0.7)',
      },
    ],
  }
})

const editorData = computed(() => {
  const entries = aggregate('editor')
  return {
    labels: entries.map(([l]) => l),
    datasets: [
      {
        label: 'Suggestions',
        data: entries.map(([, v]) => v.suggestions),
        backgroundColor: 'rgba(59, 130, 246, 0.7)',
      },
      {
        label: 'Acceptances',
        data: entries.map(([, v]) => v.acceptances),
        backgroundColor: 'rgba(16, 185, 129, 0.7)',
      },
    ],
  }
})

const chartOptions = {
  indexAxis: 'y' as const,
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { position: 'top' as const },
  },
  scales: {
    x: { beginAtZero: true },
  },
}
</script>

<template>
  <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mt-8">
    <div class="bg-white rounded-lg shadow p-6">
      <h2 class="text-lg font-semibold text-gray-700 mb-4">Suggestions &amp; Acceptances by Language</h2>
      <div class="h-80">
        <Bar :data="languageData" :options="{ ...chartOptions, plugins: { ...chartOptions.plugins, title: { display: false } } }" />
      </div>
    </div>
    <div class="bg-white rounded-lg shadow p-6">
      <h2 class="text-lg font-semibold text-gray-700 mb-4">Suggestions &amp; Acceptances by Editor</h2>
      <div class="h-80">
        <Bar :data="editorData" :options="{ ...chartOptions, plugins: { ...chartOptions.plugins, title: { display: false } } }" />
      </div>
    </div>
  </div>
</template>
