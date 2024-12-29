<template>
  <div class="locates-report">
    <div class="filter-wrapper">
      <div class="dx-fieldset">
        <div class="dx-field">
          <div class="dx-field-label">{{ LocatesColumnCaptions.accountId }}:</div>
          <div class="dx-field-value">
            <DxTextBox v-model:value="model.accountId" :show-clear-button="true" />
          </div>
        </div>
        <div class="dx-field">
          <div class="dx-field-label">{{ LocatesColumnCaptions.symbol }}:</div>
          <div class="dx-field-value">
            <DxTextBox v-model:value="model.symbol" :show-clear-button="true" :inputAttr="uppercaseTextBoxStyle"
              @value-changed="textBoxValueUpper" />
          </div>
        </div>
      </div>
      <div class="dx-fieldset">
        <div class="dx-field">
          <div class="dx-field-label">{{ LocatesColumnCaptions.source }}:</div>
          <div class="dx-field-value">
            <DxTextBox v-model:value="model.providerId" :show-clear-button="true" :inputAttr="uppercaseTextBoxStyle"
              @value-changed="textBoxValueUpper" />
          </div>
        </div>
        <div class="dx-field">
          <div class="dx-field-label">{{ LocatesColumnCaptions.status }}:</div>
          <div class="dx-field-value">
            <DxSelectBox v-model:value="model.status" :data-source="statusOptions" :show-clear-button="true" />
          </div>
        </div>
      </div>
      <div class="dx-fieldset">
        <div class="dx-field">
          <div class="dx-field-label">{{ LocatesColumnCaptions.time }} FROM:</div>
          <div class="dx-field-value">
            <DxDateBox v-model:value="model.timeFrom" type="datetime" :show-clear-button="true"
              :onOpened="setMaxDateAsToday" display-format="MM/dd/yyy HH:mm" />
          </div>
        </div>
        <div class="dx-field">
          <div class="dx-field-label">{{ LocatesColumnCaptions.time }} TO:</div>
          <div class="dx-field-value">
            <DxDateBox v-model:value="model.timeTo" type="datetime" :show-clear-button="true"
              :onOpened="setMaxDateAsToday" display-format="MM/dd/yyy HH:mm" />
          </div>
        </div>
        <div class="dx-field center">
          <DxButtonGroup :items="timeButtons" key-expr="key" selection-mode="none" @item-click="timeButtonClick" />
        </div>
      </div>
    </div>
    <div class="buttons">
      <DxButton text="Filter" type="success" @click="refreshGrid()" />
    </div>
    <div class="grid-wrapper">
      <AppTable class="fg-1" ref="appTable" :options="locatesOptions">
        <template v-slot:locate-status="{ data }">
          <LocateStatus :status="(data.data as ILocatesReportGridModel).status"
            :error="(data.data as ILocatesReportGridModel).errorMessage" />
        </template>
        <template v-slot:date-time="{ data }">
          <DateTimeTemplate :time="(data.data as ILocatesReportGridModel).time" />
        </template>
        <DxRemoteOperations :filtering="true" :paging="true" :sorting="true" />
      </AppTable>
    </div>
  </div>
</template>

<script setup lang="ts">
import { setMaxDateAsToday, textBoxValueUpper, uppercaseTextBoxStyle } from '@/@shared/utils/dx.utils';
import type { AppTableOptions } from '@/components/markup/app-table.vue';
import AppTable from '@/components/markup/app-table.vue';
import { DateFormats, nameof } from '@/constants';
import { formatQuoteSourceInfos } from '@/shared/formatters/quote-source-infos.formatter';
import type { ILocatesReportFilter } from '@/shared/models/ILocatesReportFilter';
import type { ILocatesReportGridModel } from '@/shared/models/ILocatesReportGridModel';
import { ReportEligibleLocateStatuses } from '@/shared/models/signalr';
import { reportsService } from '@/shared/services/api/reports.service';
import { authService } from '@/shared/services/auth.service';
import { LocatesColumnCaptions, LocatesReportColumnCaptions, type ReportTemplate } from '@/shared/services/auto-report.service';
import { copyToClipboard, openDetailsPage } from '@/shared/utils/quote-utils';
import dayjs from 'dayjs';
import { DxButton } from 'devextreme-vue/button';
import { DxButtonGroup } from 'devextreme-vue/button-group';
import { DxRemoteOperations } from 'devextreme-vue/data-grid';
import DxDateBox from 'devextreme-vue/date-box';
import { DxSelectBox } from 'devextreme-vue/select-box';
import { DxTextBox } from 'devextreme-vue/text-box';
import type { LoadOptions, SortDescriptor } from 'devextreme/data';
import CustomStore from 'devextreme/data/custom_store';
import { isArray } from 'lodash';
import { ref } from 'vue';
import LocateStatus from '../../institutional-locates/components/locate-status.vue';
import DateTimeTemplate from './date-time.template.vue';
import type { LocatesReportDataRequest } from '@/lib/api/v1';
import { match, P } from 'ts-pattern';

const appTable = ref<InstanceType<typeof AppTable> | null>(null);
const model = ref(setFilterToday({} as ILocatesReportFilter));
const statusOptions = ReportEligibleLocateStatuses; // add possible statuses

enum TimeButtonType { Today, Week, Month }
interface ITimeButton { text: string; key: TimeButtonType, type: 'back' | 'danger' | 'default' | 'normal' | 'success' }

const timeButtons = ref<ITimeButton[]>([
  {
    key: TimeButtonType.Today,
    text: 'Today',
    type: 'success'
  },
  {
    key: TimeButtonType.Week,
    text: 'This week',
    type: 'success'
  },
  {
    key: TimeButtonType.Month,
    text: 'This month',
    type: 'success'
  }
]);

function setFilterToday(filter: ILocatesReportFilter) {
  return {
    ...filter,
    timeFrom: dayjs().startOf('day').toDate(),
    timeTo: dayjs().toDate()
  };
}

function setFilterWeek(filter: ILocatesReportFilter) {
  return {
    ...filter,
    timeFrom: dayjs().startOf('week').toDate(),
    timeTo: dayjs().toDate()
  };
}

function setFilterMonth(filter: ILocatesReportFilter) {
  return {
    ...filter,
    timeFrom: dayjs().startOf('month').toDate(),
    timeTo: dayjs().toDate()
  };
}

function timeButtonClick(e: { itemData: ITimeButton }) {
  switch (e.itemData.key) {
    case TimeButtonType.Today:
      model.value = setFilterToday(model.value);
      break;
    case TimeButtonType.Week:
      model.value = setFilterWeek(model.value);
      break;
    case TimeButtonType.Month:
      model.value = setFilterMonth(model.value);
      break;
  }
}

function refreshGrid() {
  return appTable.value?.grid?.instance?.refresh();
}

function normalizeSort<T>(sort?: SortDescriptor<T> | Array<SortDescriptor<T>>) {
  if (!sort) {
    return [];
  }

  if (isArray(sort)) {
    return sort;
  }

  return [sort];
}

function buildParams(options?: LoadOptions<ILocatesReportGridModel>): LocatesReportDataRequest {
  let orderBy = 'time desc';
  const sort = normalizeSort(options?.sort);
  if (sort?.length) {
    const { selector, desc } = sort[0] as any;

    orderBy = `${selector} ${desc ? 'desc' : 'asc'}`;
  }

  return {
    skip: options?.skip ?? 0,
    take: options?.take ?? 2_000,
    orderBy: orderBy,
    symbol: model.value.symbol,
    status: model.value.status,
    accountId: model.value.accountId,
    from: model.value.timeFrom ? dayjs(model.value.timeFrom).utc().format(DateFormats.DAYJS_DATETIME) : null,
    to: model.value.timeTo ? dayjs(model.value.timeTo).utc().format(DateFormats.DAYJS_DATETIME) : null,
    providerId: model.value.providerId
  };
}

const locatesStore = new CustomStore<ILocatesReportGridModel, string>({
  key: [nameof<ILocatesReportGridModel>('id'), nameof<ILocatesReportGridModel>('status')],
  load: async (options: LoadOptions<ILocatesReportGridModel>) => {

    const data = match(await reportsService.getLocatesReportData(buildParams(options)))
      .with({ type: 'ok', data: P.select() }, (data) => data)
      .otherwise(() => []);

    const gridData = data.map<ILocatesReportGridModel>((locatesReportModel) => {
      return {
        ...locatesReportModel,
        formattedSources: formatQuoteSourceInfos(locatesReportModel.sources ?? [])
      };
    });

    return {
      data: gridData,

      totalCount: data.length ? data[0].totalCount : 0
    };
  }
});

const locatesOptions: AppTableOptions = {
  gridName: 'locates-history',
  dataSource: {
    store: locatesStore,
    reshapeOnPush: true
  },
  isPagingEnabled: true,
  caption: 'Locates',
  height: undefined,
  width: 1300,
  columns: [
    {
      caption: LocatesColumnCaptions['time'],
      dataField: nameof<ILocatesReportGridModel>('time'),
      sortOrder: 'desc',
      cellTemplate: 'date-time',
      width: 150,
      alignment: 'center'
    },
    {
      caption: LocatesColumnCaptions['accountId'],
      dataField: nameof<ILocatesReportGridModel>('accountId'),
      allowFiltering: true
    },
    {
      caption: LocatesColumnCaptions['symbol'],
      dataField: nameof<ILocatesReportGridModel>('symbol'),
      width: 70,
      alignment: 'center',
      allowFiltering: true
    },
    {
      caption: LocatesColumnCaptions['reqQty'],
      dataField: nameof<ILocatesReportGridModel>('reqQty'),
      alignment: 'right',
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 },
      allowFiltering: true
    },
    {
      caption: LocatesColumnCaptions['fillQty'],
      dataField: nameof<ILocatesReportGridModel>('fillQty'),
      alignment: 'right',
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 },
      allowFiltering: true
    },
    {
      caption: LocatesColumnCaptions['price'],
      dataField: nameof<ILocatesReportGridModel>('price'),
      width: 70,
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 4 },
      allowFiltering: true
    },
    {
      caption: LocatesColumnCaptions['fee'],
      dataField: nameof<ILocatesReportGridModel>('fee'),
      width: 70,
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 },
      allowFiltering: true
    },
    {
      caption: LocatesColumnCaptions['discountedPrice'],
      dataField: nameof<ILocatesReportGridModel>('discountedPrice'),
      width: 70,
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 4 },
      allowFiltering: true
    },
    {
      caption: LocatesColumnCaptions['discountedFee'],
      dataField: nameof<ILocatesReportGridModel>('discountedFee'),
      width: 70,
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 },
      allowFiltering: true
    },
    {
      caption: LocatesColumnCaptions['profit'],
      dataField: nameof<ILocatesReportGridModel>('profit'),
      width: 70,
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 0 },
      allowFiltering: true
    },
    {
      dataField: nameof<ILocatesReportGridModel>('status'),
      caption: LocatesColumnCaptions['status'],
      cellTemplate: 'locate-status',
      alignment: 'center'
    },
    {
      dataField: nameof<ILocatesReportGridModel>('formattedSources'),
      caption: LocatesColumnCaptions['source'],
      alignment: 'left',
      width: 70,
      allowSorting: true
    },
    {
      type: 'buttons',
      buttons: [
        {
          icon: 'copy',
          text: 'Copy QuoteId',
          onClick: (e) => copyToClipboard(e.row?.data?.id)
        },
        {
          icon: 'arrowright',
          text: 'Open details',
          visible: () => authService.getRoles().role == 'Admin',
          onClick: (e) => openDetailsPage(e.row?.data?.id)
        }
      ]
    }
  ],
  templates: ['locate-status', 'date-time']
};

function buildTemplate(): ReportTemplate {
  return {
    id: 0,
    name: '',
    saveToBlob: false,
    active: true,
    columns: LocatesReportColumnCaptions
  };
}
</script>
<style scoped lang="scss">
.buttons {
  display: flex;
  justify-content: center;
  margin-bottom: 30px;
  gap: 30px;
}
</style>
<style lang="scss">
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

.locates-report {
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
