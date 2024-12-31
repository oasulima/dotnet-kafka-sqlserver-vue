<template>
  <div>
    <span :id="id" :class="{ tooltip: !!error }" :style="{ color: getColor(props.status) }">{{ props.status }}</span>
    <DxPopover v-if="!!error" showEvent="mouseenter" hideEvent="mouseleave" :target="'#' + id">
      {{ error }}
    </DxPopover>
  </div>
</template>

<script setup lang="ts">
import type { QuoteResponseStatusEnum } from '@/lib/api/v1';
import DxPopover from 'devextreme-vue/popover';
import { v4 as uuid } from 'uuid';

const getColor = LocateStatusDisplayColor;
const id = 'status-' + uuid();

const props = defineProps<{
  status: QuoteResponseStatusEnum;
  error: string | null | undefined;
}>();

</script>
<script lang="ts">
function LocateStatusDisplayColor(status: QuoteResponseStatusEnum) {
  if (status == 'WaitingAcceptance' || status == 'Filled') {
    return 'green';
  }

  if (status == 'Partial') {
    return 'yellow';
  }

  return 'red';
}
</script>

<style scoped lang="scss">
.tooltip {
  border-bottom: 1px dotted grey;
}
</style>