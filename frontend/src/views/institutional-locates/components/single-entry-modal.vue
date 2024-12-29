<template>
  <AppModal :visible="props.visible" :options="{ onClose, onSubmit, isSubmitBtnDisabled, title: 'Single Entry' }">
    <div class="single-entry-modal-content">
      <DxTextBox v-model:value="model.symbol" placeholder="Symbol" @value-changed="symbolChanged"
        :inputAttr="uppercaseTextBoxStyle" />
      <DxSelectBox v-model:value="model.source" :data-source="sourcesStore" placeholder="Source" display-expr="label"
        value-expr="value" />
      <DxNumberBox v-model:value="model.quantity" placeholder="Quantity" :min="1"
        :format="{ type: 'fixedPoint', precision: 0 }" />
      <DxNumberBox v-model:value="model.price" placeholder="CPS" :format="DefaultNumberFormat" :min="0"
        :onKeyDown="applyDotOnKeyDown" />
    </div>
  </AppModal>

</template>

<script setup lang="ts">
import { applyDotOnKeyDown, textBoxValueUpper, uppercaseTextBoxStyle } from '@/@shared/utils/dx.utils';
import AppModal from '@/components/markup/app-modal.vue';
import { nameof } from '@/constants';
import type { ISingleEntryModel } from '@/shared/models/ISingleEntryModel';
import optionsService from '@/shared/services/api/options-service';
import { DxNumberBox } from 'devextreme-vue/number-box';
import { DxSelectBox } from 'devextreme-vue/select-box';
import { DxTextBox } from 'devextreme-vue/text-box';
import CustomStore from 'devextreme/data/custom_store';
import type dxTextBox from 'devextreme/ui/text_box';
import { computed, ref } from 'vue';
import { DefaultNumberFormat } from '../../settings/helpers';
import { authService } from '@/shared/services/auth.service';
import type { Subscription } from 'rxjs';
import { onMounted, onUnmounted } from 'vue';
import type { StringSelectValue } from '@/lib/api/v1';

const sourcesStore = new CustomStore<StringSelectValue, string>({
  key: nameof<StringSelectValue>('value'),
  load: async () => {
    return await optionsService.getSources();
  }
});

const props = defineProps<{ onSubmit: (x: ISingleEntryModel) => void, visible: boolean }>();
const emit = defineEmits(['update:visible']);

const model = ref(getNewModel());

function getNewModel(): ISingleEntryModel {
  return {};
}

const onClose = () => {
  emit('update:visible', false);
  model.value = getNewModel();
};

const onSubmit = () => {
  props.onSubmit(model.value);
  onClose();
};

function symbolChanged(data: { component: dxTextBox, value: string, previousValue: string }) {
  textBoxValueUpper(data);
}

const isSubmitBtnDisabled = computed(() => {
  return !model.value.symbol
    || !model.value.source
    || !(model.value.quantity && model.value.quantity > 0)
    || !(model.value.price != undefined && model.value.price >= 0);
});

</script>

<style lang="scss">
.single-entry-modal-content {
  .dx-texteditor {
    margin-bottom: 15px;
  }
}
</style>