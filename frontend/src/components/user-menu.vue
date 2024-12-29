<template>
  <DxMenu ref="menuRef" :data-source="menuItems" :display-expr="(x: any) => unwrapRef(x.name)"
    @item-click="onItemClick" />
</template>

<script setup lang="ts">
import { profileService } from '@/@shared/services/profile.service';
import { authService } from '@/shared/services/auth.service';
import DxMenu from 'devextreme-vue/menu';
import type { Subscription } from 'rxjs';
import { onMounted, onUnmounted, ref, type Ref } from 'vue';

const login = ref<string | null | undefined>('');
let loginSubscription: Subscription | undefined;
let rolesSubscription: Subscription | undefined;

const roleMenuItem = {
  name: ''
};

const menuRef = ref<DxMenu | null>(null);

onMounted(() => {
  loginSubscription = profileService.getLogin$().subscribe((x) => login.value = x);
  rolesSubscription = authService.getRoles$().subscribe((roles) => {
    if (roles.role == 'Admin') roleMenuItem.name = 'Role: Admin';
    else if (roles.role == 'Viewer') roleMenuItem.name = 'Role: Viewer';
    menuRef.value?.instance?.option('dataSource', menuItems as any);
  });
});
onUnmounted(() => {
  loginSubscription?.unsubscribe();
  rolesSubscription?.unsubscribe();
});

const menuItems = [
  {
    name: login,
    items: [
      roleMenuItem,
      {
        name: 'Sign Out',
        onClick: () => alert('Sign Out') // authService.logOut(),
      }
    ]
  }
];

function onItemClick(e: any) {
  if (e.itemData.onClick) {
    e.itemData.onClick();
  }

}

function unwrapRef(value?: string | Ref<string | null | undefined>) {
  if (!value) {
    return '';
  }

  if (typeof value === 'string') {
    return value;
  }

  return value.value;
}

</script>