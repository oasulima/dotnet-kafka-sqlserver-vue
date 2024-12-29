<template>
  <div class="app-table" :style="contentStyle">
    <div v-if="props.options.caption" class="header">
      <span>{{ props.options.caption }}</span>
      <div class="buttons">
        <slot name="custom-header-buttons"></slot>
      </div>
    </div>
    <DxDataGrid ref="grid" class="table dx-card" :data-source="props.options.dataSource" :show-borders="false"
      @context-menu-preparing="onContextMenuPreparing" :headerFilter="props.options.headerFilter"
      :rowDragging="props.options.rowDragging" :allowSorting="props.options.allowSorting" :cache-enabled="false"
      :repaint-changes-only="true" :columns="props.options.columns" @option-changed="handlePropertyChange"
      :highlight-changes="true" :focused-row-enabled="false" :column-hiding-enabled="false" :width="props.options.width"
      :height="props.options.height" :minHeight="props.options.minHeight" :editing="props.options.editing ?? {}"
      :onCellClick="props.options.onCellClick" :onEditorPreparing="props.options.onEditorPreparing"
      :onRowPrepared="props.options.onRowPrepared" :onFileSaving="props.options.onFileSaving"
      :filter-sync-enabled="true" :onExporting="props.options.onExporting" :onRowInserted="props.options.onRowInserted"
      :onInitNewRow="props.options.onInitNewRow" @row-inserting="props.options.onRowInserting"
      @row-updating="props.options.onRowUpdating" :onRowRemoved="props.options.onRowRemoved"
      @row-removing="props.options.onRowRemoving" :showColumnLines="false" :showRowLines="true"
      :allow-column-resizing="true">
      <DxExport v-bind="props.options.export"></DxExport>
      <DxPaging :page-size="props.options.pageSize ?? 50" :enabled="props.options.isPagingEnabled" />
      <DxPager :show-page-size-selector="true" :show-info="true" :allowed-page-sizes="[50, 200, 500]"
        display-mode="compact" />
      <DxFilterRow :visible="false" />
      <DxScrolling :mode="props.options.isPagingEnabled ? 'standard' : 'virtual'" column-rendering-mode="virtual" />
      <DxStateStoring :enabled="!!props.options.gridName" type="localStorage"
        :storage-key="('grid_state__' + props.options.gridName)" />
      <slot></slot>
      <template v-for="slot in (props.options.templates ?? [])" v-slot:[slot]="{ data }">
        <slot :name="slot" v-bind:data="data"></slot>
      </template>
    </DxDataGrid>
  </div>
</template>

<script setup lang="ts">

import {
  DxDataGrid, DxExport, DxFilterRow, DxStateStoring,
  DxPager, DxPaging, DxScrolling
} from 'devextreme-vue/data-grid';
import type { DataSourceOptionsStub } from 'devextreme/data/data_source';
import 'devextreme/data/odata/store';
import type { EventInfo } from 'devextreme/events';
import type { CellClickEvent, Column, Editing, EditorPreparingEvent, Export, FileSavingEvent, HeaderFilter, RowDragging, RowInsertedInfo, RowInsertingInfo, RowPreparedEvent, RowRemovedInfo } from 'devextreme/ui/data_grid';
import type { ExportingEvent } from 'devextreme/ui/pivot_grid';
import { ref } from 'vue';

export interface AppTableOptions<
  TStoreItem = any,
  TMappedItem = TStoreItem,
  TRowData = TMappedItem,
  TKey = any,
> {
  gridName?: string;
  caption?: string;
  width?: number;
  height?: number | string;
  minHeight?: number;
  dataSource: DataSourceOptionsStub;
  columns: Array<Column<TRowData, TKey> | string>;
  editing?: Editing<TRowData, TKey>;
  export?: Export;
  templates?: string[];
  allowSorting?: boolean;
  onCellClick?: ((e: CellClickEvent<TRowData, TKey>) => void);
  onRowPrepared?: ((e: RowPreparedEvent<TRowData>) => void);
  onEditorPreparing?: ((e: EditorPreparingEvent<TRowData, TKey>) => void);
  onRowInserted?: ((e: EventInfo<any> & RowInsertedInfo<TRowData, TKey>) => void);
  onRowRemoved?: ((e: EventInfo<any> & RowRemovedInfo<TRowData, TKey>) => void);
  onInitNewRow?: ((e: EventInfo<any> & RowInsertingInfo<TRowData>) => void);
  onRowInserting?: (e: { data: TRowData }) => void;
  onRowUpdating?: (e: { cancel: boolean, newData: any, oldData: any }) => void;
  onRowRemoving?: (e: { cancel: boolean, data: any }) => void;
  onExporting?: ((e: ExportingEvent) => void);
  onFileSaving?: ((e: FileSavingEvent<TRowData, TKey>) => void);
  handlePropertyChange?: (e: { name: string, fullName: string, value: string }) => void;
  rowDragging?: RowDragging<any, TRowData, TKey>;
  isPagingEnabled?: boolean;
  pageSize?: number;
  headerFilter?: HeaderFilter;
}

function onContextMenuPreparing(e: { target: string; items: any[]; }) {
  if (e.target === 'header') {
    e.items.forEach((x: { text: string; value: string; visible: boolean; }) => {
      if (x.text === 'Clear Sorting' && x.value === 'none') {
        x.visible = false;
      }
    });
  }
}

const props = defineProps<{ options: AppTableOptions }>();
const grid = ref<DxDataGrid | null>(null);
const contentStyle = [
  props.options.width ? `max-width: ${props.options.width}px;` : '',
  props.options.height ? `height: ${props.options.height}px;` : '',
  props.options.minHeight ? `min-height: ${props.options.minHeight}px;` : ''
].filter(Boolean).join('');

const reload = () => {
  void grid.value?.instance?.getDataSource().reload();
};

function refresh(changesOnly?: boolean) {
  void grid.value?.instance?.refresh(!!changesOnly);
}

function handlePropertyChange(e: any) {
  if (props.options.handlePropertyChange) {
    props.options.handlePropertyChange(e);
  }
}

defineExpose({
  grid,
  reload,
  refresh
});

</script>

<style scoped lang="scss">
@import "@/scss/variables.base.scss";

.app-table {
  display: flex;
  flex-direction: column;
  flex-grow: 1;

  .table {
    flex-grow: 1;
    flex-basis: 150px;
  }

  .header {
    padding: 6px 9px;
    background-color: #1D1D1F;
    color: #878787;
    border-radius: 10px 10px 0px 0px;
    border: 1px solid #504848;
    font-weight: 900;
    font-size: $default-font-size;
    display: flex;
    justify-content: space-between;
    align-items: center;

    .buttons {
      display: flex;
      justify-content: flex-end;
    }
  }
}
</style>

<style lang="scss">
.app-table {
  .dx-datagrid-cell-updated-animation {
    -webkit-animation: 1s dx-datagrid-highlight-change;
    animation: 1s dx-datagrid-highlight-change;
  }

  @keyframes dx-datagrid-highlight-change {

    50%,
    from {
      background-color: rgba(222, 222, 222, 0.5);
    }
  }

  .dx-datagrid {
    background-color: #292929;
    border: 1px solid #504848;
    border-top: 0;
  }

  .dx-card {
    margin: 0;
    margin-top: -3px;
    border: 0;
  }

  .dx-datagrid-headers {
    margin: 0;
    background-color: #4F5051;
    color: #E5E6E7;
    border: 0;
  }

  col {
    border: 1px solid #292929;
    border-top: 0;
  }

  .dx-datagrid .dx-datagrid-table .dx-header-row>td {
    padding: 5px 9px;
  }
}
</style>
