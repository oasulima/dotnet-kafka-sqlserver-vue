<template>
  <div class="app-badged-icon">
    <i :class="iconClass"></i>
    <div v-if="props.counter" class="badge">
      {{ props.counter }}
    </div>
  </div>
</template>
<script setup lang="ts">
const props = defineProps<{ counter: number, icon: string }>();
const iconClass = `dx-icon dx-icon-${props.icon}`;
</script>

<style lang="scss">
.app-badged-icon {
  cursor: pointer;
  position: relative;

  &>i {
    font-size: 24px;
  }

  .badge {
    background-color: red;
    color: white;
    position: absolute;
    top: -4px;
    left: 12px;
    display: inline-block;
    padding: 0px 4px;
    min-width: 8px;
    max-width: 18px;
    height: 16px;
    border: 3px solid #151516;
    border-radius: 22px;
    text-align: center;
    font-size: 12px;
    font-weight: 400;
    box-sizing: content-box;
  }
}
</style>