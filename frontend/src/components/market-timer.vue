<template>
  <div class="timer">
    <span class="text">NYSE {{ state.kind }} in </span>
    <span class="time" :class="{ 'time-closes': state.kind === 'closes' }">{{ state.timer.format('HH:mm:ss') }}</span>
  </div>
</template>

<script setup lang="ts">
import dayjs, { Dayjs } from 'dayjs';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';
import duration, { type Duration } from 'dayjs/plugin/duration';
import { onUnmounted, ref } from 'vue';

interface StockTimer {
  timer: Duration;
  kind: 'opens' | 'closes';
}

dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.extend(duration);
const nyseTimeZone = 'America/New_York';
const nyseOpenTime = dayjs.duration({ hours: 9, minutes: 30 });
const nyseCloseTime = dayjs.duration({ hours: 16 });

const state = ref(getNyseState(dayjs()));
const interval = setInterval(() => state.value = getNyseState(dayjs()), 1000);
onUnmounted(() => clearInterval(interval));

function getNyseState(time: Dayjs): StockTimer {
  const currentTime = dayjs(time).tz(nyseTimeZone);
  const currentDay = currentTime.startOf('day');
  const openTime = currentDay.add(nyseOpenTime);
  const closeTime = currentDay.add(nyseCloseTime);
  const nextDayOpenTime = openTime.add(1, 'day');

  if (currentTime.isBefore(openTime)) {
    return {
      timer: dayjs.duration(openTime.diff(currentTime)),
      kind: 'opens'
    };
  }

  if (currentTime.isBefore(closeTime)) {
    return {
      timer: dayjs.duration(closeTime.diff(currentTime)),
      kind: 'closes'
    };
  }

  return {
    timer: dayjs.duration(nextDayOpenTime.diff(currentTime)),
    kind: 'opens'
  };
}
</script>

<style scoped lang="scss">
.timer {
  display: flex;
  gap: 5px;
}

.time {
  display: block;
  width: 70px;
  color: green;

  &-closes {
    color: red;
  }
}
</style>