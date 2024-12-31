<template>
  <AppTable ref="table" :options="options">
    <DxMasterDetail :enabled="true" template="quote-details-template" />
    <template v-slot:locate-status="{ data }">
      <LocateStatus :status="(data.data as IQuoteListItem).status"
        :error="(data.data as IQuoteListItem).errorMessage" />
    </template>
    <template v-slot:quote-details-template="{ data }">
      <AppTable :options="getOptions(data.data)">
        <template v-slot:checkbox-template="{ data: d }">
          <DxCheckBox v-model:value="d.value" @disabled="(data.data as IQuoteListItem).status != 'WaitingAcceptance'"
            @value-changed="(e: any) => handlePriceSelectChanged(e, d, data)" />
        </template>
      </AppTable>
    </template>
  </AppTable>
</template>

<script setup lang="ts">
import AppTable, { type AppTableOptions } from '@/components/markup/app-table.vue';
import { nameof, SortOrderEnum } from '@/constants';
import type { ILocateGridModel } from '@/shared/models/ILocateGridModel';
import type { ILocateRequestGridModel } from '@/shared/models/ILocateRequestGridModel';
import { quoteService } from '@/shared/services/api/quote-service';
import { copyToClipboard, openDetailsPage } from '@/shared/utils/quote-utils';
import { institutionalLocatesService } from '@/views/institutional-locates/communication.service';
import DxCheckBox from 'devextreme-vue/check-box';
import { DxMasterDetail } from 'devextreme-vue/data-grid';
import ArrayStore from 'devextreme/data/array_store';
import { filter } from 'rxjs/internal/operators/filter';
import type { Subscription } from 'rxjs/internal/Subscription';
import { onMounted, onUnmounted, ref } from 'vue';
import LocateStatus from '../../institutional-locates/components/locate-status.vue';
import type { QuoteResponseStatusEnum, QuoteSourceInfo } from '@/lib/api/v1';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import { DateFormats } from '@/constants';
dayjs.extend(utc);


let locatesSubscr: Subscription | undefined;
let locateRequestsSubscr: Subscription | undefined;

const quotes: Map<string, IQuoteListItem> = new Map<string, IQuoteListItem>();

const quotesStore = new ArrayStore<IQuoteListItem>({
  key: nameof<IQuoteListItem>('id'),
  data: []
});

let refreshGridInterval: number | undefined;

onMounted(() => {
  locatesSubscr = institutionalLocatesService.$locates
    .pipe(filter((item) => item.accountId == props.accountId))
    .subscribe((locate) => {
      collapse();
      void handleLocate(locate);
    });

  locateRequestsSubscr = institutionalLocatesService.$locateRequests
    .pipe(filter((item) => item.accountId == props.accountId))
    .subscribe((locateRequest) => {
      collapse();
      void handleLocateRequest(locateRequest);
    });

  const locates = institutionalLocatesService.getLocates(props);

  for (const locate of locates) {
    void handleLocate(locate);
  }

  const locateRequests = institutionalLocatesService.getLocateRequests(props);

  for (const locateRequest of locateRequests) {
    void handleLocateRequest(locateRequest);
  }

  refreshGridInterval = setInterval(() => {
    refreshGrid();
  }, 500);
});

function refreshGrid() {
  table.value?.grid?.instance?.refresh(true);
}

function collapse() {
  table.value?.grid?.instance?.collapseAll(-1);
}

async function handleLocateRequest(locateRequest: ILocateRequestGridModel) {
  if (!quotes.has(locateRequest.id)) {
    const quote: IQuoteListItem = {
      id: locateRequest.id,
      formattedLocalTime: locateRequest.formattedLocalTime,
      utcTicks: locateRequest.utcTicks,
      symbol: locateRequest.symbol,
      qtyFill: locateRequest.qtyOffer,
      reqQty: locateRequest.qtyReq,
      price: locateRequest.price,
      status: 'WaitingAcceptance',
      formattedSources: locateRequest.formattedSources,
      sourceDetails: locateRequest.sourceDetails.map((item) => ({ ...item, selected: false })),
      accountId: locateRequest.accountId,
      errorMessage: null
    };
    await quotesStore.insert(quote);
    quotes.set(quote.id, quote);
  }
}

async function handleLocate(locate: ILocateGridModel) {
  const quote: IQuoteListItem = {
    id: locate.quoteId,
    formattedLocalTime: locate.formattedLocalTime,
    utcTicks: locate.utcTicks,
    symbol: locate.symbol,
    qtyFill: locate.qtyFill,
    reqQty: locate.reqQty,
    price: locate.price,
    status: locate.status,
    formattedSources: locate.formattedSources,
    sourceDetails: locate.sourceDetails.map((item) => ({ ...item, selected: false })),
    accountId: locate.accountId,
    errorMessage: locate.errorMessage
  };
  if (!quotes.has(locate.quoteId)) {
    await quotesStore.insert(quote);
    quotes.set(quote.id, quote);
  }
  else {
    const existingRecord = quotes.get(locate.quoteId);
    const oldStatus = existingRecord?.status;
    const newStatus = quote.status;
    if (newStatus !== 'RejectedBadRequest' && newStatus !== 'RejectedProhibited'
      || oldStatus == 'WaitingAcceptance') {

      await quotesStore.update(quote.id, quote);
      quotes.set(quote.id, quote);
    }
  }
}

interface IQuoteListItem {
  id: string;
  formattedLocalTime: string;
  utcTicks: number;
  symbol: string;
  qtyFill: number;
  reqQty: number;
  price: number;
  status: QuoteResponseStatusEnum
  formattedSources: string[];
  sourceDetails: ISelectableQuoteSourceInfo[];
  accountId: string;
  errorMessage?: string | null;
}

interface ISelectableQuoteSourceInfo extends QuoteSourceInfo {
  selected: boolean;
}

onUnmounted(() => {
  locatesSubscr?.unsubscribe();
  locateRequestsSubscr?.unsubscribe();
  clearInterval(refreshGridInterval);
});

const table = ref<InstanceType<typeof AppTable> | null>(null);
const props = defineProps<{ accountId: string; }>();

const options: AppTableOptions = {
  height: '100%',
  columns: [
    nameof<IQuoteListItem>('id'),
    {
      dataField: nameof<IQuoteListItem>('formattedLocalTime'), caption: 'Time', width: 78, alignment: 'center',
      sortOrder: SortOrderEnum.desc,
      calculateSortValue: nameof<IQuoteListItem>('utcTicks')
    },
    nameof<IQuoteListItem>('symbol'),
    nameof<IQuoteListItem>('reqQty'),
    nameof<IQuoteListItem>('qtyFill'),
    nameof<IQuoteListItem>('price'),
    {
      dataField: nameof<IQuoteListItem>('status'), caption: 'Status', cellTemplate: 'locate-status', alignment: 'center'

    },
    {
      type: 'buttons',
      buttons: [
        {
          icon: 'check',
          onClick: (e) => onAccept(e.row?.data as IQuoteListItem),
          visible: (options) => (options.row?.data as IQuoteListItem).status == 'WaitingAcceptance'
        },
        {
          icon: 'close',
          onClick: (e) => onDecline((e.row?.data as IQuoteListItem)),
          visible: (options) => (options.row?.data as IQuoteListItem).status == 'WaitingAcceptance'
        },
        {
          icon: 'copy',
          text: 'Copy QuoteId',
          onClick: (e) => copyToClipboard(e.row?.data?.id)
        },
        {
          icon: 'arrowright',
          text: 'Open details',
          onClick: (e) => openDetailsPage(e.row?.data?.id)
        }
      ]
    }
  ],
  dataSource: {
    store: quotesStore,
    reshapeOnPush: false
  },
  templates: ['quote-details-template', 'locate-status'],
  isPagingEnabled: false
};

function getOptions(data: IQuoteListItem) {
  return {
    columns: [
      nameof<ISelectableQuoteSourceInfo>('price'),
      nameof<ISelectableQuoteSourceInfo>('qty'),
      {
        dataField: nameof<ISelectableQuoteSourceInfo>('selected'),
        cellTemplate: 'checkbox-template',
        visible: data.status == 'WaitingAcceptance'
      }
    ],
    dataSource: {
      store: new ArrayStore<ISelectableQuoteSourceInfo>({
        key: nameof<ISelectableQuoteSourceInfo>('price'),
        data: data.sourceDetails
      }),
      reshapeOnPush: false
    },
    templates: ['checkbox-template'],
    isPagingEnabled: false
  };
}

async function onAccept(quote: IQuoteListItem) {
  if (quote.sourceDetails.every((x) => x.selected)
    || quote.sourceDetails.every((x) => !x.selected)) {
    await quoteService.quote({
      id: quote.id,
      accountId: quote.accountId,
      requestType: 'QuoteAccept',
      symbol: quote.symbol,
      allowPartial: false,
      autoApprove: false,
      maxPriceForAutoApprove: quote.price,
      quantity: quote.reqQty,
      time: dayjs.utc().format(DateFormats.DAYJS_DATETIME)
    });
  } else {
    const price = Math.max(...quote.sourceDetails.filter((x) => x.selected).map((x) => x.price));
    await quoteService.quote({
      id: quote.id,
      accountId: quote.accountId,
      maxPriceForAutoApprove: price,
      requestType: 'QuoteAccept',
      symbol: quote.symbol,
      allowPartial: false,
      autoApprove: false,
      quantity: quote.reqQty,
      time: dayjs.utc().format(DateFormats.DAYJS_DATETIME)
    });
  }
}

async function onDecline(quote: IQuoteListItem) {
  await quoteService.quote({
    id: quote.id,
    accountId: quote.accountId,
    requestType: 'QuoteCancel',
    symbol: quote.symbol,
    allowPartial: false,
    autoApprove: false,
    maxPriceForAutoApprove: quote.price,
    quantity: quote.reqQty,
    time: dayjs.utc().format(DateFormats.DAYJS_DATETIME)
  });
}

function handlePriceSelectChanged(e: { value: boolean }, rowData: { data: ISelectableQuoteSourceInfo, component: any }, quoteData: { data: IQuoteListItem }) {
  if (e.value === true) {
    const price = rowData.data.price;
    for (const x of quoteData.data.sourceDetails) {
      if (x.price <= price) {
        x.selected = true;
      }
    }
  } else if (e.value === false) {
    const price = rowData.data.price;
    for (const x of quoteData.data.sourceDetails) {
      if (x.price >= price) {
        x.selected = false;
      }
    }
  }
  rowData.component.refresh();
}
</script>
