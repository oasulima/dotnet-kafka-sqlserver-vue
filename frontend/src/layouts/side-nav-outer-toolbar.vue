<template>
  <div class="side-nav-outer-toolbar">
    <header-toolbar class="layout-header" :menu-toggle-enabled="true" :toggle-menu-func="toggleMenu" :title="props.title" />
    <dx-drawer class="layout-body" position="before" template="menuTemplate" v-model:opened="menuOpened"
      :opened-state-mode="drawerOptions.menuMode" :reveal-mode="drawerOptions.menuRevealMode"
      :min-size="drawerOptions.minMenuSize" :max-size="drawerOptions.maxMenuSize" :shading="drawerOptions.shaderEnabled"
      :close-on-outside-click="drawerOptions.closeOnOutsideClick">
      <dx-scroll-view ref="scrollViewRef" class="with-footer">
        <slot></slot>
      </dx-scroll-view>
      <template #menuTemplate>
        <side-nav-menu :compact-mode="!menuOpened" @click="handleSideBarClick" />
      </template>
    </dx-drawer>
  </div>
</template>

<script setup lang="ts">
import DxDrawer from 'devextreme-vue/drawer';
import DxScrollView from 'devextreme-vue/scroll-view';

import { computed, ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import HeaderToolbar from '../components/header-toolbar.vue';
import SideNavMenu from '../components/side-nav-menu.vue';

const props = defineProps<{
  title: string,
  isXSmall: boolean,
  isLarge: boolean
}>();

const route = useRoute();

const scrollViewRef = ref<DxScrollView | null>(null);
const menuOpened = ref(false);

function toggleMenu(e: any) {
  const pointerEvent = e.event;
  pointerEvent.stopPropagation();

  menuOpened.value = !menuOpened.value;
}

function handleSideBarClick() {

  menuOpened.value = true;
}

const drawerOptions = computed(() => {

  return {
    menuMode: 'overlap',
    menuRevealMode: 'expand',
    minMenuSize: 0,
    maxMenuSize: 250,
    closeOnOutsideClick: true,
    shaderEnabled: true
  };
});

watch(
  () => route.path,
  () => {
    menuOpened.value = false;
    scrollViewRef.value?.instance?.scrollTo(0);
  }
);

</script>

<style lang="scss">
.side-nav-outer-toolbar {
  flex-direction: column;
  display: flex;
  height: 100%;
  width: 100%;
}

.layout-header {
  z-index: 1501;
}

.layout-body {
  flex: 1;
  min-height: 0;
}
</style>
