<template>
  <div>
    {{ formattedTime }}
  </div>
</template>

<script setup lang="ts">


import { DateFormats } from '@/constants';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import { computed } from 'vue';
dayjs.extend(utc);

const props = defineProps<{
  time: string
}>();

const formattedTime = computed(() => {
  return dayjs.utc(props.time).local().format(DateFormats.DAYJS_DATETIME);
});

</script>
