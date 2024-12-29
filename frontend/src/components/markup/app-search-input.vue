<template>
  <div class="app-seach-input">
    <DxTextBox :width="props.width" :placeholder="props.placeholder" class="app-override-search" :value="props.value"
      :show-clear-button="false" mode="search" @input="onInput" />
  </div>

</template>

<script setup lang="ts">
import { DxTextBox } from 'devextreme-vue/text-box';

const props = defineProps<{ width?: number, value: string, placeholder?: string }>();
const emit = defineEmits(['update:value']);

function onInput(e: any) {
  emit('update:value', e.event.originalEvent.target.value);
}
</script>

<style lang="scss">
.app-seach-input .dx-texteditor {
  &.dx-editor-filled {

    .dx-placeholder,
    .dx-texteditor-input {
      padding-left: 30px;
    }
  }
}
</style>
