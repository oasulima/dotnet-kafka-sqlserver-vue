<template>
  <div>
    <AppGridWithLinkButton ref="gridRef" :options="inventoryDatabaseOptions"
      :isLinked$="communication.$isInventoryDatabaseLinked" class="inventory-database-grid" />
  </div>
</template>

<script setup lang="ts">
import type { AppTableOptions } from '@/components/markup/app-table.vue';
import { nameof } from '@/constants';
import type { IInventoryDatabaseModel } from '@/shared/models/IInventoryDatabaseModel';
import { internalInventoryService } from '@/shared/services/api/internal-inventory-service';
import { requiredRule } from '@/views/settings/helpers';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import type dxDataGrid from 'devextreme/ui/data_grid';
import type { Column, Row, RowPreparedEvent } from 'devextreme/ui/data_grid';
import type { Subscription } from 'rxjs/internal/Subscription';
import { onMounted, onUnmounted, ref } from 'vue';
import { institutionalLocatesService as communication } from '../communication.service';
import AppGridWithLinkButton from './app-grid-with-link-button.vue';
import { authService } from '@/shared/services/auth.service';
import { combineLatest } from 'rxjs';

interface InternalInventorySortModel {
  historyItem: boolean,
  sortValue: any
}

dayjs.extend(utc);

const gridRef = ref<InstanceType<typeof AppGridWithLinkButton> | null>(null);


let getInfoSubscr: Subscription | undefined;
let rolesSubscription: Subscription | undefined;

onMounted(() => {
  getInfoSubscr = combineLatest([communication.$isInventoryDatabaseLinked, communication.$symbolFilter]).subscribe(([isLinked, symbol]) => {
    communication.reloadInventoryGrid({ isLinked, symbol });
  });

  rolesSubscription = authService.getRoles$().subscribe((roles) => {
    allowEditing.value = roles.role != 'Viewer';
    buttonsColumn.visible = allowEditing.value;
    const grid = gridRef?.value?.getDxComponent();
    grid?.columnOption(buttonsColumn.name!, 'visible', buttonsColumn.visible);
  });
});

function shelf(data: IInventoryDatabaseModel) {
  if (communication.startDeactivating(data.Id)) {
    void internalInventoryService.deactivate(data.Item);
  }
}

function activate(data: IInventoryDatabaseModel) {
  if (communication.startActivating(data.Id)) {
    void internalInventoryService.activate(data.Item);
  }
}

const isRowInactive = (options: { component?: dxDataGrid<IInventoryDatabaseModel, string> | undefined; row?: Row<IInventoryDatabaseModel, string> | undefined; }): boolean => {
  const inventoryItem = (options.row?.data as IInventoryDatabaseModel);
  return !inventoryItem.updating && allowEditing.value && communication.isIIItemInactive(inventoryItem);
};

const isRowActive = (options: { component?: dxDataGrid<IInventoryDatabaseModel, string> | undefined; row?: Row<IInventoryDatabaseModel, string> | undefined; }): boolean => {
  const inventoryItem = (options.row?.data as IInventoryDatabaseModel);
  return !inventoryItem.updating && communication.isIIItemActive(inventoryItem);
};

const isShowLoadingIcon = (options: { component?: dxDataGrid<IInventoryDatabaseModel, string> | undefined; row?: Row<IInventoryDatabaseModel, string> | undefined; }): boolean => {
  const inventoryItem = (options.row?.data as IInventoryDatabaseModel);
  return inventoryItem.updating == true;
};

const roles = authService.getRoles();
const allowEditing = ref(roles.role != 'Viewer');

const buttonsColumn: Column<IInventoryDatabaseModel, string> = {
  name: 'buttonsColumn',
  visible: allowEditing.value,
  type: 'buttons',
  width: 100,
  buttons: [
    'edit',
    'delete',
    { hint: 'Activate', icon: 'upload', visible: isRowInactive, onClick: (e) => activate(e.row!.data) },
    { hint: 'Shelf', icon: 'download', visible: isRowActive, onClick: (e) => shelf(e.row!.data) },
    { hint: 'Loading...', icon: 'clock', visible: isShowLoadingIcon }
  ],
  alignment: 'right'
};

const inventoryDatabaseOptions: AppTableOptions = {
  gridName: 'institutional-locates/inventory-database',
  dataSource: {
    store: communication.inventoryDatabaseStore,
    reshapeOnPush: true,
    pushAggregationTimeout: 500
  },
  isPagingEnabled: false,
  caption: 'Inventory Database',
  height: '100%',
  minHeight: 200,
  columns: [
    {
      dataField: nameof<IInventoryDatabaseModel>('FormattedLocalDate'),
      caption: 'Date',
      width: 96,
      allowEditing: false,
      alignment: 'center',
      calculateSortValue: getCalculateSortValueMethod('UTCTicks'),
      sortingMethod: getSortingMethod('FormattedLocalDate')
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('FormattedLocalTime'),
      sortOrder: 'desc',
      caption: 'Time',
      width: 78,
      allowEditing: false,
      alignment: 'center',
      calculateSortValue: getCalculateSortValueMethod('UTCTicks'),
      sortingMethod: getSortingMethod('FormattedLocalTime')
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('Symbol'),
      caption: 'Symbol',
      width: 70,
      allowEditing: false,
      alignment: 'center',
      calculateSortValue: getCalculateSortValueMethod('Symbol'),
      sortingMethod: getSortingMethod('Symbol')
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('Qty'),
      caption: 'Qty',
      allowEditing: false,
      alignment: 'right',
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 },
      calculateSortValue: getCalculateSortValueMethod('Qty'),
      sortingMethod: getSortingMethod('Qty')
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('Filled'),
      caption: 'Filled',
      allowEditing: false,
      alignment: 'right',
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 },
      calculateSortValue: getCalculateSortValueMethod('Filled'),
      sortingMethod: getSortingMethod('Filled')
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('Avl'),
      caption: 'Avl',
      validationRules: [requiredRule], editorOptions: {
        format: { type: 'fixedPoint', precision: 0 },
        min: 0
      },
      alignment: 'right',
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 },
      calculateSortValue: getCalculateSortValueMethod('Avl'),
      sortingMethod: getSortingMethod('Avl')
    },
    {
      caption: 'CPS',
      dataField: nameof<IInventoryDatabaseModel>('Rate'),
      validationRules: [requiredRule],
      editorOptions: {
        format: { type: 'fixedPoint', precision: 4 },
        min: 0
      },
      width: 70, alignment: 'right',
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 4 },
      calculateSortValue: getCalculateSortValueMethod('Rate'),
      sortingMethod: getSortingMethod('Rate')
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('CreatingType'),
      caption: 'Creating Type',
      allowEditing: false,
      calculateSortValue: getCalculateSortValueMethod('CreatingType'),
      sortingMethod: getSortingMethod('CreatingType')
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('Source'),
      caption: 'Source',
      alignment: 'left',
      width: 70,
      allowSorting: true,
      calculateSortValue: getCalculateSortValueMethod('Source'),
      sortingMethod: getSortingMethod('Source')
    },
    buttonsColumn
  ],
  templates: [],
  editing: {
    allowDeleting: isRowInactive,
    allowUpdating: isRowInactive,
    allowAdding: false,
    useIcons: true,
    mode: 'row'
  },
  onRowPrepared: (e: RowPreparedEvent<IInventoryDatabaseModel, string>) => {
    if (e.data?.HistoryItem) {
      e.rowElement.style.opacity = '0.5';
    }
  },
  onRowUpdating: async (e: { cancel: boolean, newData: { Avl?: number; Rate?: number; }, oldData: IInventoryDatabaseModel }) => {
    if (e.newData.Avl != undefined || e.newData.Rate != undefined) {
      e.cancel = true;
      const updatedItem = e.oldData.Item;
      if (e.newData.Avl) {
        updatedItem.quantity = e.newData.Avl;
      }
      if (e.newData.Rate) {
        updatedItem.price = e.newData.Rate;
      }
      gridRef.value?.getDxComponent()?.cancelEditData();
      if (communication.startUpdatingII(updatedItem.id)) {
        await internalInventoryService.update(updatedItem);
      }
    }
  },
  onRowRemoving: async (e: { cancel: boolean, data: IInventoryDatabaseModel }) => {
    e.cancel = true;
    if (communication.startUpdatingII(e.data.Id)) {
      await internalInventoryService.delete(e.data.Item);
    }
  }
};

function getCalculateSortValueMethod(colDataField: keyof IInventoryDatabaseModel) {
  const calculateSortValueMethod = (data: IInventoryDatabaseModel): InternalInventorySortModel => {
    return { historyItem: data.HistoryItem, sortValue: data[colDataField] };
  };
  return calculateSortValueMethod;
}

function getSortingMethod(colDataField: keyof IInventoryDatabaseModel) {
  const sortingMethod = (a: InternalInventorySortModel, b: InternalInventorySortModel) => {
    const sortOrder = getCurrentSortOrder(colDataField);

    if (a.historyItem !== b.historyItem) {
      if (a.historyItem) return sortOrder * 1;
      if (b.historyItem) return sortOrder * -1;
    }

    if (a.sortValue < b.sortValue) return -1;
    if (a.sortValue > b.sortValue) return 1;
    return 0;
  };

  return sortingMethod;
}

function getCurrentSortOrder(colDataField: keyof IInventoryDatabaseModel) {
  const gridColumns = gridRef.value?.getDxComponent()?.state().columns as Column[];
  const sortOrder = gridColumns.filter((x) => x.dataField === colDataField)[0].sortOrder;
  return sortOrder === 'asc' ? 1 : -1;
}

onUnmounted(() => {
  rolesSubscription?.unsubscribe();
  getInfoSubscr?.unsubscribe();

});

</script>

<style scoped lang="scss">
.inventory-database-grid {
  height: 100%;
}
</style>