<template>
  <header class="header-component">
    <dx-toolbar class="header-toolbar">
      <dx-item :visible="menuToggleEnabled" location="before" css-class="menu-button">
        <template #default>
          <dx-button icon="menu" styling-mode="text" @click="toggleMenuFunc" />
        </template>
      </dx-item>

      <dx-item v-if="title" location="before">
        <div class="logo-title">{{ title }}</div>
      </dx-item>

      <dx-item location="center">
        <span class="current-page-title">
          {{ $route.meta.pageHeader }}
        </span>
      </dx-item>

      <dx-item location="after">
        <MarketTimer />
      </dx-item>

      <dx-item location="after">
        <AppNotificationPopover />
      </dx-item>

      <dx-item location="after">
        <UserMenu />
      </dx-item>
    </dx-toolbar>
  </header>
</template>

<script setup lang="ts">
import DxButton from 'devextreme-vue/button';
import DxToolbar, { DxItem } from 'devextreme-vue/toolbar';
import AppNotificationPopover from './notifications/app-notification-popover.vue';
import UserMenu from './user-menu.vue';
import MarketTimer from './market-timer.vue';

defineProps<{
  menuToggleEnabled: boolean,
  title: string,
  toggleMenuFunc: (e: any) => void
}>();

</script>

<style lang="scss">
@import "../scss/variables.base.scss";

.header-component {
  .header-toolbar {
    background-color: #151516;
    height: 56px;
  }

  flex: 0 0 auto;
  z-index: 1;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12),
  0 1px 2px rgba(0, 0, 0, 0.24);

  .logo-title {
    font-size: $header-font-size;
  }

  .current-page-title {
    color: #42C482;
    font-weight: 900;
    font-size: $caption-font-size;
  }

  .dx-toolbar .dx-toolbar-item.menu-button>.dx-toolbar-item-content .dx-icon {
    color: $base-accent;
  }
}

.header-component .dx-toolbar.header-toolbar .dx-toolbar-items-container .dx-toolbar-after {
  padding: 0 40px;

  .screen-x-small & {
    padding: 0 20px;
  }
}

.header-component .dx-toolbar .dx-toolbar-item.dx-toolbar-button.menu-button {
  width: $side-panel-min-width;
  text-align: center;

  padding: 0;

  & .dx-button-content {
    display: flex;
    align-items: center;
  }
}

.dx-theme-generic .header-component {
  .dx-toolbar {
    padding: 10px 0;
  }

  .user-button>.dx-button-content {
    padding: 3px;
  }
}
</style>
