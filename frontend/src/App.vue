<script setup lang="ts">
import { sizes, subscribe, unsubscribe } from './utils/media-query';
import {
  reactive,
  onMounted,
  onBeforeUnmount,
  computed,
  ref
} from 'vue';
import { applicationService } from '@/shared/services/api/application-service';

function getScreenSizeInfo() {
  const screenSizes = sizes();
  const classes: string[] = [
    screenSizes['screen-large'] ? 'screen-large' : '',
    screenSizes['screen-medium'] ? 'screen-medium' : '',
    screenSizes['screen-small'] ? 'screen-small' : '',
    screenSizes['screen-x-small'] ? 'screen-x-small' : ''

  ];

  return {
    isXSmall: screenSizes['screen-x-small'],
    isLarge: screenSizes['screen-large'],
    cssClasses: classes
  };
}

const screen = reactive({ getScreenSizeInfo: getScreenSizeInfo() });

const Title = 'TEST SITE';

const title = ref<string>(Title);

function screenSizeChanged() {
  screen.getScreenSizeInfo = getScreenSizeInfo();
}

onMounted(async () => {
  subscribe(screenSizeChanged);
  const data = await applicationService.getRunningEnvName();
  if (data) {
    title.value = `${Title} ${data}`;
  }
});

onBeforeUnmount(() => {
  unsubscribe(screenSizeChanged);
});

const cssClasses = computed(() => {
  return ['app'].concat(screen.getScreenSizeInfo.cssClasses);
});

</script>

<template>
  <div id="root">
    <div :class="cssClasses">
      <component :is="$route.meta.layout" :title="title" :is-x-small="screen.getScreenSizeInfo.isXSmall"
        :is-large="screen.getScreenSizeInfo.isLarge">
        <div class="app-main-content">
          <router-view></router-view>
        </div>
      </component>
    </div>
  </div>
</template>


<style lang="scss">
html,
body {
  margin: 0px;
  min-height: 100%;
  height: 100%;
}

#root {
  height: 100%;
}

* {
  box-sizing: border-box;
}

.app {
  @import "./scss/variables.base.scss";
  display: flex;
  height: 100%;
  width: 100%;
}

.app-main-content {
  flex-grow: 1;
  height: 1px;
  margin: 10px;
}
</style>
