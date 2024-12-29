<template>
  <AppGridWithLinkButton ref="gridRef" :options="locatesOptions" :isLinked$="communication.$isLocatesLinked">
    <template v-slot:locate-status="{ data }">
      <LocateStatus :status="(data.data as ILocateGridModel).status"
        :error="(data.data as ILocateGridModel).errorMessage" />
    </template>
  </AppGridWithLinkButton>
</template>

<script setup lang="ts">
import type { AppTableOptions } from '@/components/markup/app-table.vue';
import { nameof } from '@/constants';
import type { ILocateGridModel } from '@/shared/models/ILocateGridModel';
import { LocatesColumnCaptions } from '@/shared/services/auto-report.service';
import { combineLatest } from 'rxjs';
import type { Subscription } from 'rxjs/internal/Subscription';
import { onMounted, onUnmounted, ref } from 'vue';
import { institutionalLocatesService as communication } from '../communication.service';
import LocateStatus from '../components/locate-status.vue';
import AppGridWithLinkButton from './app-grid-with-link-button.vue';
import { applyDxGridExactColumnFilter } from '@/@shared/utils/dx.utils';

const gridRef = ref<InstanceType<typeof AppGridWithLinkButton> | null>(null);

let filterSubscr: Subscription | undefined;

onMounted(() => {
  filterSubscr = combineLatest([communication.$isLocatesLinked, communication.$symbolFilter])
    .subscribe(([isLinked, symbol]) => {
      const component = gridRef.value?.getDxComponent();
      const column = nameof<ILocateGridModel>('symbol');
      const value = isLinked && symbol ? symbol : undefined;
      applyDxGridExactColumnFilter(component, column, value);
    });
});


const locatesOptions: AppTableOptions = {
  gridName: 'institutional-locates/locates',
  dataSource: {
    store: communication.locatesStore,
    reshapeOnPush: true,
    pushAggregationTimeout: 500
  },
  isPagingEnabled: false,
  caption: 'Locates',
  height: '100%',
  minHeight: 200,
  headerFilter: {
    visible: true
  },
  columns: [
    {
      caption: LocatesColumnCaptions.time,
      sortOrder: 'desc',
      width: 78,
      allowHeaderFiltering: false,
      dataField: nameof<ILocateGridModel>('formattedLocalTime'),
      calculateSortValue: nameof<ILocateGridModel>('utcTicks')
    },
    {
      caption: LocatesColumnCaptions.accountId,
      allowHeaderFiltering: false,
      dataField: nameof<ILocateGridModel>('accountId')
    },
    {
      caption: LocatesColumnCaptions.symbol,
      width: 70,
      allowHeaderFiltering: false,
      dataField: nameof<ILocateGridModel>('symbol')
    },
    {
      caption: 'Qty',
      allowHeaderFiltering: false,
      format: { type: 'fixedPoint', precision: 0 },
      dataField: nameof<ILocateGridModel>('qtyFill')
    },
    {
      caption: LocatesColumnCaptions.price,
      width: 70,
      format: { type: 'fixedPoint', precision: 4 },
      allowHeaderFiltering: false,
      dataField: nameof<ILocateGridModel>('price')
    },
    {
      caption: LocatesColumnCaptions.fee,
      dataField: nameof<ILocateGridModel>('fee'),
      allowHeaderFiltering: false,
      format: { type: 'fixedPoint', precision: 0 }
    },
    {
      caption: LocatesColumnCaptions.discountedPrice,
      width: 80,
      format: { type: 'fixedPoint', precision: 4 },
      allowHeaderFiltering: false,
      dataField: nameof<ILocateGridModel>('discountedPrice')
    },
    {
      caption: LocatesColumnCaptions.discountedFee,
      dataField: nameof<ILocateGridModel>('cost'),
      allowHeaderFiltering: false,
      format: { type: 'fixedPoint', precision: 0 }
    },
    {
      caption: LocatesColumnCaptions.profit,
      dataField: nameof<ILocateGridModel>('pnl'),
      allowHeaderFiltering: false,
      format: { type: 'fixedPoint', precision: 0 }
    },
    {
      caption: LocatesColumnCaptions.status,
      minWidth: 90,
      cellTemplate: 'locate-status',
      allowHeaderFiltering: true,
      allowSorting: true,
      dataField: nameof<ILocateGridModel>('status')
    },
    {
      caption: LocatesColumnCaptions.source,
      alignment: 'left',
      headerFilter: {
        dataSource: communication.locatesProviderIdsArr
      },
      width: 70,
      allowSorting: false,
      dataField: nameof<ILocateGridModel>('formattedSources')
    }
  ],
  templates: ['locate-status']
};

onUnmounted(() => {
  filterSubscr?.unsubscribe();
});
</script>
