<template>
  <AppPageWrapper :maxWidth="1000">
    <DxButton text="Refresh" type="success" @click="reload" />
    <div class="table-wrapper">
      <AppSettingTable ref="table" :options="options">
        <template v-slot:health-cell="{ data }">

          <div v-if="data.data.autoDisabled">
            <span v-if="!allowEditing" class="dx-icon-close"></span>
            <a v-if="allowEditing" class="dx-link" style="text-decoration: none;"
              @click="activateProvider(data.data.providerId)"
              :title="`Disabled for ${data.data.autoDisabled.symbols ? data.data.autoDisabled.symbols.join(', ') : 'all'} symbol${!data.data.autoDisabled.symbols || data.data.autoDisabled.symbols.length > 1 ? 's' : ''}`">Activate</a>
          </div>
          <div v-else>
            <div class="dx-widget dx-state-readonly dx-checkbox dx-checkbox-checked dx-datagrid-checkbox-size"
              aria-readonly="true" aria-invalid="false" role="checkbox" aria-checked="true">
              <div class="dx-checkbox-container"><span class="dx-checkbox-icon"></span></div>
            </div>
          </div>

        </template>

      </AppSettingTable>
    </div>
  </AppPageWrapper>
</template>

<script setup lang="ts">
import { applyDotOnKeyDown } from '@/@shared/utils/dx.utils';
import AppPageWrapper from '@/components/wrappers/app-page-wrapper.vue';
import { providerService } from '@/shared/services/api/provider-service';
import { onMounted, onUnmounted, ref } from 'vue';
import AppSettingTable, { type AppSettingTableOptions } from './components/app-setting-table.vue';
import { DefaultNumberFormat, requiredRule } from './helpers';
import { DxButton } from 'devextreme-vue/button';
import { authService } from '@/shared/services/auth.service';
import type { Subscription } from 'rxjs/internal/Subscription';
import { ProvidersStore } from '@/utils/providers-store';
import { nameof } from '@/constants';
import type { ProviderSettingExtended } from '@/lib/api/v1';

const table = ref<InstanceType<typeof AppSettingTable> | null>(null);

const store = ProvidersStore();

function reload() {
  table?.value?.table?.refresh();
}

function activateProvider(providerId: string) {
  providerService.activateProvider(providerId).then(() =>
    setTimeout(() => {
      reload();
    }, 500)
  ).catch(() => reload());
}

const allowEditing = ref(authService.getRoles().role != 'Viewer');

let rolesSubscription: Subscription | undefined;

onMounted(() => {
  rolesSubscription = authService.getRoles$().subscribe((roles) => {
    allowEditing.value = roles.role != 'Viewer';
  });
});

onUnmounted(() => {
  rolesSubscription?.unsubscribe();
});

const options: AppSettingTableOptions = {
  onEditorPreparing: (e) => {
    if (e.dataField === 'providerId' && e.row?.data['providerId']) {
      e.editorOptions.readOnly = true;
    }
  },
  onInitNewRow: (e) => e.data['active'] = false,
  columns: [
    {
      dataField: nameof<ProviderSettingExtended>('name'),
      sortOrder: 'asc',
      validationRules: [requiredRule],
      editorOptions: {
        maxLength: 100
      }
    },
    {
      dataField: nameof<ProviderSettingExtended>('providerId'),
      caption: 'ID',
      validationRules: [requiredRule],
      editorOptions: {
        maxLength: 100
      }
    },
    {
      dataField: nameof<ProviderSettingExtended>('discount'),
      dataType: 'number',
      validationRules: [requiredRule],
      format: DefaultNumberFormat,
      editorOptions: {
        onKeyDown: applyDotOnKeyDown,
        showClearButton: true,
        format: DefaultNumberFormat,
        min: 0,
        max: 0.9999
      }
    },
    {
      dataField: nameof<ProviderSettingExtended>('active'),
      dataType: 'boolean'
    },
    {
      caption: 'Health',
      alignment: 'center',
      cellTemplate: 'health-cell',
      allowEditing: false
    }
  ],
  dataSource: {
    store,
    reshapeOnPush: false
  },
  templates: ['health-cell']
};

</script>
<style scoped lang="scss">
.table-wrapper {
  display: flex;
  flex-wrap: wrap;
  gap: 30px;
  margin-top: 20px;
  justify-content: space-around;
  height: 100%;
}
</style>