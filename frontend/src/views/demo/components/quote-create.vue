<template>
  <div class="quote-create">
    <DxTextBox v-model:value="model.symbol" valueChangeEvent="input" placeholder="Symbol"
      :inputAttr="uppercaseTextBoxStyle" @value-changed="textBoxValueUpper" />
    <DxNumberBox v-model:value="model.quantity" valueChangeEvent="input" placeholder="Quantity" />
    <div class="check-box-item">
      <DxCheckBox v-model:value="model.allowPartial" />
      <span>Allow Partial</span>
    </div>
    <div class="check-box-item">
      <DxCheckBox v-model:value="model.autoApprove" />
      <span>Auto Approve</span>
    </div>
    <DxNumberBox v-if="model.autoApprove" v-model:value="model.maxPriceForAutoApprove" valueChangeEvent="input"
      placeholder="Auto Approve Max Price" />
    <DxButton text="Quote" :disabled="isInvalid" type="success" @click="onQuote" />
  </div>
</template>
<script setup lang="ts">
import { textBoxValueUpper, uppercaseTextBoxStyle } from '@/@shared/utils/dx.utils';
import { DxButton } from 'devextreme-vue/button';
import { DxCheckBox } from 'devextreme-vue/check-box';
import { DxNumberBox } from 'devextreme-vue/number-box';
import { DxTextBox } from 'devextreme-vue/text-box';
import { computed, ref } from 'vue';

export interface QuoteCreateModel {
  symbol: string;
  quantity: number;
  allowPartial: boolean;
  autoApprove: boolean;
  maxPriceForAutoApprove: number;
}

const props = defineProps<{ onSubmit: (x: QuoteCreateModel) => void }>();
const model = ref({ allowPartial: false, autoApprove: false } as QuoteCreateModel);
const isInvalid = computed(() => {
  const { symbol, quantity, autoApprove, maxPriceForAutoApprove } = model.value;

  return !symbol || !quantity || (autoApprove && !maxPriceForAutoApprove);
});

const onQuote = () => {
  props.onSubmit(model.value);
  model.value = { allowPartial: false, autoApprove: false } as QuoteCreateModel;
};
</script>

<style lang="scss">
.quote-create {
  display: flex;

  >* {
    margin-right: 10px;
    width: 262px;
  }

  .check-box-item {
    display: flex;
    align-items: center;
    color: rgb(229, 230, 231);
    font-size: 16px;

    .dx-checkbox {
      margin-right: 10px;
    }
  }
}
</style>
