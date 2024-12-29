<template>
  <div class="app-setting-table">
    <div class="tool-panel">
      <AppSearchInput class="search-input" placeholder="Filter" v-model:value="filterValue"
        @update:value="onSearchUpdate()" />
      <IconAddButton class="add-button" v-on:click="onAddClicked" v-if="options.editing && allowEditing" />
    </div>
    <AppTable class="mt-10 fg-1" ref="table" :options="options">
      <template v-for="slot in (props.options.templates ?? [])" v-slot:[slot]="{ data }">
        <slot :name="slot" v-bind:data="data"></slot>
      </template>
    </AppTable>
  </div>
</template>

<script setup lang="ts">
import IconAddButton from '@/components/icons/IconAddButton.vue';
import AppSearchInput from '@/components/markup/app-search-input.vue';
import AppTable, { type AppTableOptions } from '@/components/markup/app-table.vue';
import { authService } from '@/shared/services/auth.service';
import type { Subscription } from 'rxjs';
import { onMounted, onUnmounted, ref } from 'vue';

export type AppSettingTableOptions = Pick<AppTableOptions, 'editing' | 'gridName' | 'headerFilter' | 'dataSource' | 'columns' | 'onCellClick' | 'onEditorPreparing' | 'onRowInserting' | 'onInitNewRow' | 'templates' | 'caption'>;


const table = ref<InstanceType<typeof AppTable> | null>(null);
const props = defineProps<{ options: AppSettingTableOptions }>();
const filterValue = ref('');
defineExpose({
  table
});

const allowEditing = ref(authService.getRoles().role == 'Admin');

let rolesSubscription: Subscription | undefined;

onMounted(() => {
  rolesSubscription = authService.getRoles$().subscribe((role) => {
    allowEditing.value = role.role == 'Admin';
    if (options.editing) {
      options.editing.allowDeleting = role.role == 'Admin';
      options.editing.allowUpdating = role.role == 'Admin';
      table?.value?.grid?.instance?.option('editing', options.editing);
    }
  });
});

onUnmounted(() => {
  rolesSubscription?.unsubscribe();
});

const options: AppTableOptions = {
  isPagingEnabled: true,
  editing: {
    allowDeleting: allowEditing.value,
    allowUpdating: allowEditing.value,
    useIcons: true
  },
  ...props.options
};

function onSearchUpdate() {
  table?.value?.grid?.instance?.searchByText(filterValue.value);
}

function onAddClicked() {
  table?.value?.grid?.instance?.addRow();
}

</script>

<style lang="scss">
.app-setting-table {
  display: flex;
  flex-direction: column;
  align-items: center;
  flex-grow: 1;

  .tool-panel {
    display: flex;
    width: 100%;

    .search-input {
      width: 100%;
    }

    .add-button {
      margin-left: 10px;
      cursor: pointer;
    }
  }
}
</style>
