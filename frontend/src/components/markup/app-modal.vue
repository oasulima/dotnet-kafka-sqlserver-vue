<template>
  <DxPopup :visible="props.visible" :onHidden="onHidden" :drag-enabled="false" :height="props.options.height ?? 'auto'"
    :width="props.options.width" :close-on-outside-click="!props.options.disableBackdropClick" :show-close-button="true"
    :show-title="true" :title="props.options.title">
    <div class="app-modal-content">
      <div class="content">
        <slot></slot>
      </div>
      <div class="footer">
        <DxButton v-if="props.options.onSubmit" :disabled="props.options.isSubmitBtnDisabled" text="SUBMIT"
          type="success" @click="props.options.onSubmit" />
        <DxButton v-if="props.options.onBack" text="BACK" type="success" stylingMode="outlined"
          @click="props.options.onBack" />
        <DxButton v-if="props.options.onCancel" text="CANCEL" type="success" stylingMode="outlined"
          @click="props.options.onCancel" />
      </div>
    </div>
  </DxPopup>
</template>

<script setup lang="ts">
import { DxPopup } from 'devextreme-vue/popup';
import { DxButton } from 'devextreme-vue/button';

export type AppModalOptions = {
  title: string;
  height?: number;
  width?: number;
  onClose?: () => void;
  onSubmit?: () => void;
  onBack?: () => void;
  onCancel?: () => void;
  disableBackdropClick?: boolean;
  isSubmitBtnDisabled?: boolean | (() => boolean);
};

const props = defineProps<{ options: AppModalOptions, visible: boolean }>();
const emit = defineEmits(['update:visible']);
const onHidden = () => {
  if (props.options?.onClose) {
    props.options.onClose();
  }

  emit('update:visible', false);
};
</script>

<style lang="scss">
.app-modal-content {
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: space-between;

  .footer {
    display: flex;
    justify-content: center;

    >* {
      margin-top: 5px;
      margin: 5px;
    }
  }
}
</style>
