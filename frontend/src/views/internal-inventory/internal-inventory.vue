<template>
  <div class="internal-inventory">
    <div class="filter-wrapper">
      <div class="dx-fieldset">
        <div class="dx-field">
          <div class="dx-field-label">Symbol:</div>
          <div class="dx-field-value">
            <DxTextBox v-model:value="model.symbol" :show-clear-button="true" :inputAttr="uppercaseTextBoxStyle"
              @value-changed="textBoxValueUpper" />
          </div>
        </div>

        <div class="dx-field">
          <div class="dx-field-label">Creating Type:</div>
          <div class="dx-field-value">
            <DxSelectBox v-model:value="model.creatingType" :data-source="creatingTypesStore" :show-clear-button="true"
              display-expr="label" value-expr="value" />
          </div>
        </div>
      </div>

      <div class="dx-fieldset">

        <div class="dx-field">
          <div class="dx-field-label">Status:</div>
          <div class="dx-field-value">
            <DxSelectBox v-model:value="model.status" :data-source="statusesStore" :show-clear-button="true"
              display-expr="label" value-expr="value" />
          </div>
        </div>

      </div>
    </div>
    <div class="buttons">
      <DxButton text="Filter" type="success" @click="refreshGrid()" />
    </div>
    <div class="grid-wrapper">
      <AppTable class="fg-1" ref="appTable" :options="locatesOptions"></AppTable>
    </div>
  </div>
</template>

<script setup lang="ts">
import { textBoxValueUpper, uppercaseTextBoxStyle } from '@/@shared/utils/dx.utils';
import type { AppTableOptions } from '@/components/markup/app-table.vue';
import AppTable from '@/components/markup/app-table.vue';
import { nameof } from '@/constants';
import type { IInternalInventoryFilter } from '@/shared/models/IInternalInventoryFilter';
import type { IInventoryDatabaseModel } from '@/shared/models/IInventoryDatabaseModel';
import type { ISelectValue } from '@/shared/models/ISelectValue';
import optionsService from '@/shared/services/api/options-service';
import { DxButton } from 'devextreme-vue/button';
import { DxSelectBox } from 'devextreme-vue/select-box';
import { DxTextBox } from 'devextreme-vue/text-box';
import CustomStore from 'devextreme/data/custom_store';
import { ref } from 'vue';
import { internalInventoryService } from '@/shared/services/api/internal-inventory-service';
import { toInventoryDatabaseModel } from '@/shared/mappers/inventory-database-model.mapper';
import { match, P } from 'ts-pattern';

const appTable = ref<InstanceType<typeof AppTable> | null>(null);
const model = ref({} as IInternalInventoryFilter);

const creatingTypesStore = new CustomStore<ISelectValue<string>, string>({
  key: nameof<ISelectValue<string>>('value'),
  load: async () => {
    const creatingTypes = await optionsService.getCreatingTypes();
    return match(creatingTypes)
      .returnType<ISelectValue<string>[]>()
      .with({ type: 'ok', data: P.select() }, (data) => data.map((x) => ({ value: x, label: x })))
      .otherwise(() => []);
  }
});

const statusesStore = new CustomStore<ISelectValue<string>, string>({
  key: nameof<ISelectValue<string>>('value'),
  load: async () => {
    const statuses = await optionsService.getInternalInventoryStatuses();
    return match(statuses)
      .returnType<ISelectValue<string>[]>()
      .with({ type: 'ok', data: P.select() }, (data) => data.map((x) => ({ value: x, label: x })))
      .otherwise(() => []);
  }
});

function refreshGrid() {
  return appTable.value?.grid?.instance?.refresh();
}

const internalInventoryItemsStore = new CustomStore<IInventoryDatabaseModel, string>({
  key: nameof<IInventoryDatabaseModel>('Id'),
  load: async () => {
    const items = await internalInventoryService.getItems(model.value) ?? [];
    const gridData = match(items)
      .returnType<IInventoryDatabaseModel[]>()
      .with({ type: 'ok', data: P.select() }, (data) => data.map((x) => toInventoryDatabaseModel(x, false)))
      .otherwise(() => []);

    return {
      data: gridData,
      totalCount: gridData.length
    };
  }
});

const locatesOptions: AppTableOptions = {
  gridName: 'internal-inventory',
  dataSource: {
    store: internalInventoryItemsStore,
    reshapeOnPush: true
  },
  isPagingEnabled: true,
  caption: 'Inventory Database',
  height: undefined,
  width: 1300,
  columns: [
    {
      dataField: nameof<IInventoryDatabaseModel>('FormattedLocalDate'),
      caption: 'Date',
      width: 96,
      allowEditing: false,
      alignment: 'center'
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('FormattedLocalTime'),
      sortOrder: 'desc',
      caption: 'Time',
      width: 78,
      allowEditing: false,
      alignment: 'center'
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('Symbol'),
      caption: 'Symbol',
      width: 70,
      allowEditing: false,
      alignment: 'center'
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('Qty'),
      caption: 'Qty.',
      allowEditing: false,
      alignment: 'right',
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 }
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('Filled'),
      caption: 'Filled',
      allowEditing: false,
      alignment: 'right',
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 }
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('Avl'),
      caption: 'Avl.',
      alignment: 'right',
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 }
    },
    {
      caption: 'CPS',
      dataField: nameof<IInventoryDatabaseModel>('Rate'),
      width: 70, alignment: 'right',
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 4 }
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('CreatingType'),
      caption: 'Creating Type',
      allowEditing: false
    },
    {
      dataField: nameof<IInventoryDatabaseModel>('Source'),
      caption: 'Source',
      alignment: 'left',
      width: 70,
      allowSorting: true
    },
    {
      calculateCellValue: (data: IInventoryDatabaseModel) => { return data.Item.status; },
      caption: 'Status',
      alignment: 'left',
      width: 70,
      allowSorting: true
    }
  ]
};
</script>

<style scoped lang="scss">
.buttons {
  display: flex;
  justify-content: center;
  margin-bottom: 30px;
  gap: 30px;
}

.dx-fieldset {
  .dx-field {
    width: 400px;

    &.center {
      text-align: center;
    }
  }

  .dx-field-label {
    color: rgb(229, 230, 231);
    font-size: 14px;
  }
}

.app-override-search.dx-texteditor {
  &.dx-editor-filled {

    .dx-placeholder,
    .dx-texteditor-input {
      padding-left: 30px;
    }
  }
}

.internal-inventory {
  height: 100%;
  display: flex;
  flex-direction: column;

  .filter-wrapper {
    display: flex;
    justify-content: center;
  }

  .grid-wrapper {
    display: flex;
    justify-content: center;
    flex-grow: 1;
  }
}

.dx-buttongroup-item.dx-button.dx-button-mode-contained:not(.dx-item-selected).dx-button-success {
  color: #42C482;
  min-width: unset;
}
</style>
