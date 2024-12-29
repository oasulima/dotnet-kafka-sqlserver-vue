<template>
  <AppGridWithLinkButton ref="gridRef" :options="mostLocatedOptions" :isLinked$="communication.$isMostLocatedLinked" />
</template>

<script setup lang="ts">
import type { AppTableOptions } from '@/components/markup/app-table.vue';
import { nameof } from '@/constants';
import type { IMostLocatedModel } from '@/shared/models/IMostLocatedModel';
import { combineLatest } from 'rxjs';
import type { Subscription } from 'rxjs/internal/Subscription';
import { onMounted, onUnmounted, ref } from 'vue';
import { institutionalLocatesService as communication } from '../communication.service';
import AppGridWithLinkButton from './app-grid-with-link-button.vue';
import { applyDxGridExactColumnFilter } from '@/@shared/utils/dx.utils';

const gridRef = ref<InstanceType<typeof AppGridWithLinkButton> | null>(null);

let filterSubscr: Subscription | undefined;

onMounted(() => {
  filterSubscr = combineLatest([communication.$isMostLocatedLinked, communication.$symbolFilter])
    .subscribe(([isLinked, symbol]) => {
      const component = gridRef.value?.getDxComponent();
      const column = nameof<IMostLocatedModel>('Symbol');
      const value = isLinked && symbol ? symbol : undefined;
      applyDxGridExactColumnFilter(component, column, value);
    });
});

const mostLocatedOptions: AppTableOptions = {
  gridName: 'institutional-locates/most-located',
  dataSource: {
    store: communication.mostLocatedStore,
    reshapeOnPush: true,
    pushAggregationTimeout: 500
  },
  isPagingEnabled: false,
  caption: 'Most located',
  height: '100%',
  minHeight: 200,
  columns: [
    {
      dataField: nameof<IMostLocatedModel>('Symbol'), caption: 'Symbol', width: 85, alignment: 'center', sortOrder: 'asc'
    },
    {
      dataField: nameof<IMostLocatedModel>('Requests'), caption: 'Requests', alignment: 'right', dataType: 'number', format: { type: 'fixedPoint', precision: 0 }
    },
    {
      dataField: nameof<IMostLocatedModel>('ReqQty'), caption: 'ReqQty', alignment: 'right', dataType: 'number', format: { type: 'fixedPoint', precision: 0 }
    },
    {
      dataField: nameof<IMostLocatedModel>('Fills'), caption: 'Fills', alignment: 'right', dataType: 'number', format: { type: 'fixedPoint', precision: 0 }
    },
    {
      dataField: nameof<IMostLocatedModel>('FillQty'), caption: 'FillQty', alignment: 'right', dataType: 'number', format: { type: 'fixedPoint', precision: 0 }
    },
    {
      dataField: nameof<IMostLocatedModel>('Cancels'), caption: 'Cancels', alignment: 'right', dataType: 'number', format: { type: 'fixedPoint', precision: 0 }
    }
  ]
};

onUnmounted(() => {
  filterSubscr?.unsubscribe();
});

</script>
