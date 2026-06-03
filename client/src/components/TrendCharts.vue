<script setup lang="ts">
import { computed } from 'vue'
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js'
import type { DailyMetric } from '../types'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend)

const props = defineProps<{ metrics: DailyMetric[] }>()

const sorted = computed(() =>
  [...props.metrics].sort((a, b) => a.date.localeCompare(b.date))
)

const labels = computed(() => sorted.value.map((m) => m.date))

const acceptanceRateData = computed(() => ({
  labels: labels.value,
  datasets: [
    {
      label: 'Acceptance Rate (%)',
      data: sorted.value.map((m) =>
        m.total_suggestions_count === 0
          ? 0
          : (m.total_acceptances_count / m.total_suggestions_count) * 100
      ),
      borderColor: '#3b82f6',
      backgroundColor: 'rgba(59,130,246,0.1)',
      tension: 0.3,
    },
  ],
}))

const activeUsersData = computed(() => ({
  labels: labels.value,
  datasets: [
    {
      label: 'Active Users',
      data: sorted.value.map((m) => m.total_active_users),
      borderColor: '#10b981',
      backgroundColor: 'rgba(16,185,129,0.1)',
      tension: 0.3,
    },
  ],
}))

const suggestionsAcceptancesData = computed(() => ({
  labels: labels.value,
  datasets: [
    {
      label: 'Suggestions',
      data: sorted.value.map((m) => m.total_suggestions_count),
      borderColor: '#8b5cf6',
      backgroundColor: 'rgba(139,92,246,0.1)',
      tension: 0.3,
    },
    {
      label: 'Acceptances',
      data: sorted.value.map((m) => m.total_acceptances_count),
      borderColor: '#f59e0b',
      backgroundColor: 'rgba(245,158,11,0.1)',
      tension: 0.3,
    },
  ],
}))

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { position: 'top' as const } },
  scales: { x: { title: { display: true, text: 'Date' } } },
}
</script>

<template>
  <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mt-8">
    <div class="bg-white rounded-lg shadow p-6 lg:col-span-2">
      <h2 class="text-lg font-semibold text-gray-700 mb-4">Acceptance Rate Over Time</h2>
      <div class="h-64">
        <Line :data="acceptanceRateData" :options="chartOptions" />
      </div>
    </div>
    <div class="bg-white rounded-lg shadow p-6">
      <h2 class="text-lg font-semibold text-gray-700 mb-4">Active Users Over Time</h2>
      <div class="h-64">
        <Line :data="activeUsersData" :options="chartOptions" />
      </div>
    </div>
    <div class="bg-white rounded-lg shadow p-6">
      <h2 class="text-lg font-semibold text-gray-700 mb-4">Suggestions & Acceptances Over Time</h2>
      <div class="h-64">
        <Line :data="suggestionsAcceptancesData" :options="chartOptions" />
      </div>
    </div>
  </div>
</template>
