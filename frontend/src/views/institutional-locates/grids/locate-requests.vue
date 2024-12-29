<template>
  <AppGridWithLinkButton ref="gridRef" :options="locateRequestsOptions"
    :isLinked$="communication.$isLocateRequestsLinked" />
</template>

<script setup lang="ts">

import type { AppTableOptions } from '@/components/markup/app-table.vue';
import { nameof } from '@/constants';
import type { ILocateRequestGridModel } from '@/shared/models/ILocateRequestGridModel';
import { combineLatest } from 'rxjs';
import type { Subscription } from 'rxjs/internal/Subscription';
import { onMounted, onUnmounted, ref } from 'vue';
import { institutionalLocatesService as communication } from '../communication.service';
import AppGridWithLinkButton from './app-grid-with-link-button.vue';
import { applyDxGridExactColumnFilter } from '@/@shared/utils/dx.utils';

const gridRef = ref<InstanceType<typeof AppGridWithLinkButton> | null>(null);

let filterSubscr: Subscription | undefined;

onMounted(() => {
  filterSubscr = combineLatest([communication.$isLocateRequestsLinked, communication.$symbolFilter])
    .subscribe(([isLinked, symbol]) => {
      const component = gridRef.value?.getDxComponent();
      const column = nameof<ILocateRequestGridModel>('symbol');
      const value = isLinked && symbol ? symbol : undefined;
      applyDxGridExactColumnFilter(component, column, value);
    });
});

const locateRequestsOptions: AppTableOptions = {
  gridName: 'institutional-locates/locate-requests',
  dataSource: {
    store: communication.locateRequestsStore,
    reshapeOnPush: true,
    pushAggregationTimeout: 500
  },
  isPagingEnabled: false,
  caption: 'Locate requests',
  height: '100%',
  minHeight: 200,
  columns: [
    {
      dataField: nameof<ILocateRequestGridModel>('formattedLocalTime'), caption: 'Time', width: 78, alignment: 'center',
      sortOrder: 'desc',
      calculateSortValue: nameof<ILocateRequestGridModel>('utcTicks')
    },
    {
      dataField: nameof<ILocateRequestGridModel>('accountId'), caption: 'Acct'
    },

    {
      dataField: nameof<ILocateRequestGridModel>('symbol'), caption: 'Symbol', width: 70, alignment: 'center'
    },
    {
      dataField: nameof<ILocateRequestGridModel>('qtyReq'), caption: 'QtyReq', alignment: 'right', dataType: 'number', format: { type: 'fixedPoint', precision: 0 }
    },
    {
      dataField: nameof<ILocateRequestGridModel>('qtyOffer'), caption: 'QtyOff', alignment: 'right', dataType: 'number', format: { type: 'fixedPoint', precision: 0 }
    },
    {
      dataField: nameof<ILocateRequestGridModel>('price'), caption: 'FPS', width: 70, dataType: 'number', format: { type: 'fixedPoint', precision: 4 }
    },
    {
      dataField: nameof<ILocateRequestGridModel>('discountedPrice'),
      caption: 'CPS',
      width: 70,
      dataType: 'number',
      format: { type: 'fixedPoint', precision: 4 }
    },
    {
      dataField: nameof<ILocateRequestGridModel>('formattedSources'),
      caption: 'Source',
      alignment: 'left',
      width: 70,
      allowSorting: false
    }
  ],
  templates: []
};

onUnmounted(() => {
  filterSubscr?.unsubscribe();
});

</script>
