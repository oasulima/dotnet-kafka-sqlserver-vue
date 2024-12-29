<template>
  <div class="institutional-locates">
    <div class="items items_grow">
      <locateRequestsVue class="locate-requests" />
      <locatesVue class="locates" />
      <mostLocatedVue class="most-located" />
    </div>
    <div class="items items_fixed">
      <div class="availability">
        <div v-if="symbolAvalability" class="availability__symbol">{{ symbolFilter }}</div>
        <div v-if="symbolAvalability" class="availability__type">&nbsp;is&nbsp;{{ symbolAvalability }}</div>
      </div>
      <DxTextBox width="285px" placeholder="Symbol" v-model="symbolFilter" :show-clear-button="false" mode="search"
        @enter-key="onSymbolFilterEnter" :inputAttr="uppercaseTextBoxStyle"
        :onValueChanged="() => symbolAvalability = undefined" />
      <DxButton v-if="allowSingleEntry" text="Single Entry" type="success" @click="singleEntryModalVisible = true" />
      <DxButton text="Toggle view" type="success" stylingMode="outlined" @click="toggleView" />
    </div>
    <div class="items items_grow" v-show="showAllTables">
      <inventoryDatabaseVue class="inventory-database" />
    </div>
  </div>

  <SingleEntryModal v-model:visible="singleEntryModalVisible" :onSubmit="singleEntryModalSubmit" />
</template>

<script setup lang="ts">
import type { IInventoryItem } from '@/shared/models/internal-inventory/IInventoryEntryApiModel';
import type { ISingleEntryModel } from '@/shared/models/ISingleEntryModel';
import { internalInventoryService } from '@/shared/services/api/internal-inventory-service';
import { DxButton } from 'devextreme-vue/button';
import { DxTextBox } from 'devextreme-vue/text-box';
import { institutionalLocatesService as communication } from './communication.service';
import SingleEntryModal from './components/single-entry-modal.vue';
import inventoryDatabaseVue from './grids/inventory-database.vue';
import locateRequestsVue from './grids/locate-requests.vue';
import locatesVue from './grids/locates.vue';
import mostLocatedVue from './grids/most-located.vue';
import { uppercaseTextBoxStyle } from '@/@shared/utils/dx.utils';
import { authService } from '@/shared/services/auth.service';
import { type Subscription } from 'rxjs';
import { onMounted, onUnmounted, ref } from 'vue';

const singleEntryModalVisible = ref(false);
const showAllTables = ref(true);
const symbolFilter = ref(communication.$symbolFilter.value);
const symbolAvalability = ref(undefined as string | undefined);

const roles = authService.getRoles();
const allowSingleEntry = ref(roles.role != 'Viewer');

let rolesSubscription: Subscription | undefined;

onMounted(() => {
  rolesSubscription = authService.getRoles$().subscribe((roles) => {
    allowSingleEntry.value = roles.role != 'Viewer';
  });
});

onUnmounted(() => {
  rolesSubscription?.unsubscribe();
});

const singleEntryModalSubmit = async (x: ISingleEntryModel) => {
  await internalInventoryService.add(x as IInventoryItem);
};

function toggleView() {
  showAllTables.value = !showAllTables.value;
}

function onSymbolFilterEnter() {
  communication.$symbolFilter.next(symbolFilter.value.toUpperCase());
}

</script>
<style scoped lang="scss">
.locate-requests {
  flex: 1 1 500px;
}

.availability {
  width: 200px;
  display: flex;
  justify-content: flex-end;

  .availability__symbol {
    flex-shrink: 1;
    text-transform: uppercase;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .availability__type {
    flex-shrink: 0;
  }
}

.locates {
  flex: 1 1 600px;
}

.most-located {
  flex: 1 1 400px;
  max-width: 550px;
}

.inventory-database {
  flex: 1 1 750px;
}

.external-providers {
  flex: 1 1 250px;
}

.institutional-locates {
  display: flex;
  flex-direction: column;
  height: 100%;
  gap: 20px;

  .items {
    display: flex;
    justify-content: center;
    gap: 10px;
    flex-wrap: wrap;

    &.items_grow {
      flex-grow: 1;
    }

    &.items_fixed {
      align-items: center;
    }
  }
}
</style>
