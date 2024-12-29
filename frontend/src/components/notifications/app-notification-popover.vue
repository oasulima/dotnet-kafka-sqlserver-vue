<template>
  <div class="app-message-popover">
    <AppBadgedIcon :counter="counter" icon="warning" @click="toggleVisibility" />
  </div>
  <DxPopover :width="500" :maxHeight="600" :visible="isVisible" target=".app-message-popover .app-badged-icon"
    position="top">
    <div class="app-message-popover-content">
      <DxScrollView v-if="isVisible" width="100%" height="100%">
        <AppNotificationList :items="items" />
      </DxScrollView>
    </div>
  </DxPopover>
</template>

<script setup lang="ts">
import AppBadgedIcon from './app-badged-icon.vue';
import { notificationService, type GroupedNotification } from '@/shared/services/notification.service';
import DxPopover from 'devextreme-vue/popover';
import DxScrollView from 'devextreme-vue/scroll-view';
import { onMounted, onUnmounted, ref } from 'vue';
import AppNotificationList from './app-notification-list.vue';

const isVisible = ref(false);
const counter = ref(0);
const items = ref([] as readonly GroupedNotification[]);

function toggleVisibility() {
  isVisible.value = !isVisible.value;

  if (isVisible.value) {
    items.value = notificationService.getAll();

    if (counter.value) {
      counter.value = 0;
      notificationService.markAllViewed();
    }
  }
}

function reload() {
  counter.value = notificationService.getAll().reduce((a, b) => a + (b.isViewed ? 0 : 1), 0);
}

const subscription = notificationService.getUpdateStream().subscribe(() => reload());
onMounted(() => reload());
onUnmounted(() => subscription.unsubscribe());
</script>

<style lang="scss">
.app-message-popover-content {
  height: 100%;
}
</style>