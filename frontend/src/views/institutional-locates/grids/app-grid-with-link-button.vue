<template>
  <AppTable ref="appTableRef" :options="props.options">
    <template v-slot:custom-header-buttons>
      <IconLinkTable class="link-button" :class="{ 'link-button_active': isLinked }" @click="onLinkButtonClick" />
    </template>
    <template v-for="slot in (props.options.templates ?? [])" v-slot:[slot]="{ data }">
      <slot :name="slot" v-bind:data="data"></slot>
    </template>
    <slot></slot>
  </AppTable>
</template>

<script setup lang="ts">
import IconLinkTable from '@/components/icons/IconLinkTable.vue';
import AppTable from '@/components/markup/app-table.vue';
import type { AppTableOptions } from '@/components/markup/app-table.vue';
import type { BehaviorSubject, Subscription } from 'rxjs';
import { onMounted, onUnmounted, ref } from 'vue';

const appTableRef = ref<InstanceType<typeof AppTable> | null>(null);
const props = defineProps<{ options: AppTableOptions, isLinked$: BehaviorSubject<boolean> }>();
const isLinked = ref(false);

let isLinkedSubscription: Subscription | undefined;
onMounted(() => isLinkedSubscription = props.isLinked$.subscribe((x) => isLinked.value = x));
onUnmounted(() => isLinkedSubscription?.unsubscribe());

function onLinkButtonClick() {
  props.isLinked$.next(!isLinked.value);
}

defineExpose({
  getDxComponent: () => appTableRef?.value?.grid?.instance
});
</script>

<style scoped lang="scss">
@import "@/scss/variables.base.scss";

.link-button {
  cursor: pointer;

  &_active {
    color: $success-color;
  }
}
</style>