<template>
  <div class="app-page-wrapper">
    <div class="content" :style="contentStyle">
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">

const props = defineProps<{ maxWidth?: number, marginTop?: number }>();
const contentStyle = [
  props.maxWidth ? `max-width: min(${props.maxWidth}px, 100%);` : '',
  props.marginTop ? `margin-top: ${props.marginTop}px;` : ''
].filter((x) => x).join(' ');

</script>

<style scoped lang="scss">
.app-page-wrapper {
  display: flex;
  justify-content: center;
  height: 100%;

  .content {
    display: flex;
    flex-direction: column;
    align-items: center;
    flex-grow: 1;
  }
}
</style>
