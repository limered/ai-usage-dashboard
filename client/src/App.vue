<script setup lang="ts">
import { computed } from 'vue'
import { useMetrics } from './composables/useMetrics'

const { data, loading, error } = useMetrics()

const latest = computed(() => {
  if (!data.value.length) return null
  return data.value.reduce((a, b) => (a.date > b.date ? a : b))
})

const acceptanceRate = computed(() => {
  if (!latest.value || latest.value.total_suggestions_count === 0) return 0
  return (latest.value.total_acceptances_count / latest.value.total_suggestions_count) * 100
})
</script>

<template>
  <div class="min-h-screen bg-gray-100 p-8">
    <h1 class="text-3xl font-bold text-gray-800 mb-8">AI Usage Dashboard</h1>

    <div v-if="loading" class="text-gray-600">Loading...</div>

    <div v-else-if="error" class="text-red-600 bg-red-50 p-4 rounded">
      Error: {{ error }}
    </div>

    <div v-else-if="latest" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-sm text-gray-500 uppercase tracking-wide">Acceptance Rate</p>
        <p class="mt-2 text-3xl font-bold text-gray-900">{{ acceptanceRate.toFixed(1) }}%</p>
      </div>
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-sm text-gray-500 uppercase tracking-wide">Active Users</p>
        <p class="mt-2 text-3xl font-bold text-gray-900">{{ latest.total_active_users }}</p>
      </div>
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-sm text-gray-500 uppercase tracking-wide">Total Suggestions</p>
        <p class="mt-2 text-3xl font-bold text-gray-900">{{ latest.total_suggestions_count.toLocaleString() }}</p>
      </div>
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-sm text-gray-500 uppercase tracking-wide">Total Acceptances</p>
        <p class="mt-2 text-3xl font-bold text-gray-900">{{ latest.total_acceptances_count.toLocaleString() }}</p>
      </div>
    </div>
  </div>
</template>
