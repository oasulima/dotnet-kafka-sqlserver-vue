<template>
  <div class="app-message-list">
    <div class="message" v-bind:class="{ unread: !item.isViewed }" v-for="(item, index) in props.items" :key="index">
      <div class="message-icon">
        <i v-if="item.type === 'Warning'" class="dx-icon dx-icon-warning yellow"></i>
        <i v-if="item.type === 'Error'" class="dx-icon dx-icon-clear orange"></i>
        <i v-if="item.type === 'Critical'" class="dx-icon dx-icon-clear red"></i>
      </div>
      <div class="message-content">
        <div class="message-text">{{ item.lastMessage }}</div>
        <div class="message-details">
          <span class="message-details-section">{{ dayjs(item.lastTime).format('MMM DD, HH:mm:ss') }}</span>
          <span class="message-details-section" v-if="(item.count > 1)"> and {{ (item.count - 1) }} more, since {{
            dayjs(item.firstTime).format('MMM DD, HH:mm:ss') }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ViewedGroupedNotification } from '@/shared/services/notification.service';
import dayjs from 'dayjs';

const props = defineProps<{ items: readonly ViewedGroupedNotification[] }>();
</script>

<style lang="scss">
.app-message-list {
  .message {
    color: gray;

    &.unread {
      font-weight: bold;
      color: white;
    }

    &:not(:first-child) {
      margin-top: 5px;
    }

    &:not(:last-child) {
      padding-bottom: 5px;
      border-bottom: 1px solid gray;
    }

    display: flex;
    justify-content: stretch;

    .message-icon {
      width: 40px;
      flex-shrink: 0;
      min-height: 30px;
      display: flex;
      justify-content: space-around;
      align-items: center;

      .yellow {
        color: yellow;
      }

      .orange {
        color: orange;
      }

      .red {
        color: red;
      }
    }

    .message-content {
      width: 100%;
      overflow: hidden;

      .message-text {
        text-overflow: ellipsis;
        overflow: hidden;
      }

      .message-details {
        .message-details-section {
          font-size: 11px;
        }

        opacity: 0.5;
        margin-top: 5px;
      }
    }
  }
}
</style>