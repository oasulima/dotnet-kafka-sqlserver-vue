<template>
  <div class="dx-swatch-additional side-navigation-menu" @click="forwardClick">
    <slot></slot>
    <div class="menu-container">
      <dx-tree-view ref="treeViewRef" :items="items" key-expr="path" selection-mode="single"
        :focus-state-enabled="false" expand-event="click" @item-click="handleItemClick" width="100%" />
    </div>
  </div>
</template>

<script setup lang="ts">
import DxTreeView from 'devextreme-vue/ui/tree-view';
import { onMounted, ref, watch, onUnmounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { menuItems, type IMenuItem } from '@/router';
import { authService } from '@/shared/services/auth.service';
import type { Subscription } from 'rxjs';
import type { UserRoleEnum } from '@/constants';

const props = defineProps<{
  compactMode: boolean
}>();


const route = useRoute();
const router = useRouter();

const items = getUserMenuItems(authService.getRoles().role);

function getUserMenuItems(userRole: UserRoleEnum) {
  return menuItems.filter((x) => (x as IMenuItem).meta.allowedRoles.indexOf(userRole) > -1);
}

const treeViewRef = ref<DxTreeView | null>(null);

function forwardClick(...args: any[]) {
  // context.emit("click", args);
}

function handleItemClick(e: any) {
  if (!e.itemData.path || props.compactMode) {
    return;
  }
  void router.push(e.itemData.path);

  const pointerEvent = e.event;
  pointerEvent.stopPropagation();
}

function updateSelection() {
  if (!treeViewRef.value || !treeViewRef.value.instance) {
    return;
  }

  treeViewRef.value.instance.selectItem(route.path);
  void treeViewRef.value.instance.expandItem(route.path);
}

let rolesSubscription: Subscription | undefined;


onUnmounted(() => {
  rolesSubscription?.unsubscribe();
});

onMounted(() => {
  rolesSubscription = authService.getRoles$().subscribe((roles) => {
    treeViewRef.value?.instance?.option('items', getUserMenuItems(roles.role));
  });
  updateSelection();
  if (props.compactMode) {
    treeViewRef.value?.instance?.collapseAll();
  }
});


watch(
  () => route.path,
  () => {
    updateSelection();
  }
);

watch(
  () => props.compactMode,
  () => {
    if (props.compactMode) {
      treeViewRef.value?.instance?.collapseAll();
    } else {
      updateSelection();
    }
  }
);


</script>

<style lang="scss">
@import "../scss/variables.base.scss";

.side-navigation-menu {
  display: flex;
  flex-direction: column;
  min-height: 100%;
  height: 100%;
  width: 250px !important;

  .menu-container {
    min-height: 100%;
    display: flex;
    flex: 1;

    .dx-treeview {
      // ## Long text positioning
      white-space: nowrap;
      // ##

      // ## Icon width customization
      .dx-treeview-item {
        padding: 10px 0;

        .dx-icon {
          width: $side-panel-min-width !important;
          margin: 0 !important;
        }
      }

      // ##

      // ## Arrow customization
      .dx-treeview-node {
        padding: 0 0 !important;
      }

      .dx-treeview-toggle-item-visibility {
        right: 10px;
        left: auto;
      }

      // ##

      // ## Item levels customization
      .dx-treeview-node {
        &[aria-level="1"] {
          font-weight: bold;
          border-bottom: 1px solid $base-border-color;
        }

        &[aria-level="2"] .dx-treeview-item-content {
          font-weight: normal;
          padding: 0 $side-panel-min-width;
        }
      }

      // ##
    }

    // ## Selected & Focuced items customization
    .dx-treeview {
      .dx-treeview-node-container {
        .dx-treeview-node {
          &.dx-state-selected:not(.dx-state-focused)>.dx-treeview-item {
            background: transparent;
          }

          &.dx-state-selected>.dx-treeview-item * {
            color: $base-accent;
          }

          &:not(.dx-state-focused)>.dx-treeview-item.dx-state-hover {
            background-color: lighten($base-bg, 4);
          }
        }
      }
    }

    .dx-theme-generic .dx-treeview {
      .dx-treeview-node-container .dx-treeview-node.dx-state-selected.dx-state-focused>.dx-treeview-item * {
        color: inherit;
      }
    }

    // ##
  }
}
</style>
