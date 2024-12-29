<template>
  <AppTable class="fg-1" ref="table" :options="options" />
</template>

<script setup lang="ts">
import AppTable, { type AppTableOptions } from '@/components/markup/app-table.vue';
import { onUpdated, ref } from 'vue';
import { BuildLoadOnlyStore } from '@/utils/data-stores';
import { inventoryService } from '../inventory.service';
import { nameof } from '@/constants';
import type { InventoryItem } from '@/lib/api/v1';

const table = ref<InstanceType<typeof AppTable> | null>(null);
const props = defineProps<{ accountId: string; }>();
const reload = () => table.value?.reload();

defineExpose({ reload });
onUpdated(() => reload());

const options: AppTableOptions = {
  height: '100%',
  columns: [
    {
      dataField: nameof<InventoryItem>('symbol'),
      sortOrder: 'asc',
      caption: 'Symbol'
    },
    nameof<InventoryItem>('locatedQuantity'),
    nameof<InventoryItem>('availableQuantity')
  ],
  dataSource: {
    store: BuildLoadOnlyStore(() => inventoryService.get({ accountId: props.accountId }), 'id')
  },
  isPagingEnabled: false
};
</script>

<style lang="scss"></style>
