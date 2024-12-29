import { take } from 'rxjs';
import { createRouter, createWebHashHistory, type RouteRecordRaw } from 'vue-router';
import type { UserRoleEnum } from './constants';
import defaultLayout from './layouts/side-nav-outer-toolbar.vue';
import { authService } from './shared/services/auth.service';
import QuotePage from './views/demo/quote-page.vue';
import InstitutionalLocates from './views/institutional-locates/institutional-locates.vue';
import LocatesReportPage from './views/reports/locates/locates.report.vue';
import ProvidersPage from './views/settings/providers-page.vue';
import InternalInventoryPage from './views/internal-inventory/internal-inventory.vue';

// authService.tryLoginFromLocationHref();

export interface IMenuItem {
  text: string;
  path: string;
  icon: string;
  meta: {
    allowedRoles: UserRoleEnum[]
  }
}

export const menuItems: (RouteRecordRaw & IMenuItem)[] = [
  {
    text: 'Institutional locates',
    path: '/institutional-locates',
    icon: 'dragvertical',
    name: 'institutional-locates',
    meta: {
      layout: defaultLayout,
      pageHeader: 'Institutional Locates',
      allowedRoles: ['Admin', 'Viewer', 'Unknown']
    },
    component: InstitutionalLocates
  },
  {
    text: 'Providers',
    path: '/settings/providers',
    icon: 'dragvertical',
    name: 'settings-providers',
    meta: {
      layout: defaultLayout,
      pageHeader: 'Providers',
      allowedRoles: ['Admin', 'Viewer']
    },
    component: ProvidersPage
  },
  {
    text: 'Demo Quote',
    path: '/demo/quote',
    icon: 'dragvertical',
    name: 'demo-quote',
    meta: {
      layout: defaultLayout,
      pageHeader: 'Demo Quote',
      allowedRoles: ['Admin']
    },
    component: QuotePage
  },
  {
    text: 'Locates History',
    path: '/reports/locates',
    icon: 'dragvertical',
    name: 'locates-report',
    meta: {
      layout: defaultLayout,
      pageHeader: 'Locates History',
      allowedRoles: ['Admin', 'Viewer']
    },
    component: LocatesReportPage
  },
  {
    text: 'Internal Inventory',
    path: '/internal-inventory',
    icon: 'dragvertical',
    name: 'internal-inventory',
    meta: {
      layout: defaultLayout,
      pageHeader: 'Internal Inventory',
      allowedRoles: ['Admin']
    },
    component: InternalInventoryPage
  }
];

const router = createRouter({
  routes: [
    ...menuItems as RouteRecordRaw[],
    {
      path: '/',
      redirect: '/institutional-locates'
    },
    {
      path: '/recovery',
      redirect: '/institutional-locates'
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/institutional-locates'
    }
  ],
  history: createWebHashHistory()
});

router.beforeEach((to, _from, next) => {
  // if (!authService.isAuthenticatied()) {
  //   return authService.logOut();
  // }

  const menuItem = to as {
    meta?: {
      allowedRoles?: UserRoleEnum[]
    }
  };
  authService.getRoles$().pipe(take(1)).subscribe((role) => {
    if (menuItem.meta?.allowedRoles?.indexOf(role.role) == -1) {
      next({ name: 'institutional-locates' });
    }
    else {
      next();
    }
  });

});

export default router;
