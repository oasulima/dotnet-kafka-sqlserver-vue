import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

// https://vitejs.dev/config/
export default defineConfig(({ command, mode }) => {
  const baseUrl = /* mode === 'production' ? '/v1/locator-admin-ui/' :*/ '/';
  return {
    build: {
      target: 'esnext'
    },
    base: baseUrl,
    server: {
      port: 8000
    },
    plugins: [
      vue()
    ],
    resolve: {
      alias: {
        'devextreme/ui': 'devextreme/esm/ui',
        '@': fileURLToPath(new URL('./src', import.meta.url))
      }
    }
  };
});