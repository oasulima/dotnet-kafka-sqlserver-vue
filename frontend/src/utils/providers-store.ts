import { nameof } from '@/constants';
import type { ProviderSettingExtended, ProviderSettingRequest } from '@/lib/api/v1';
import { client } from '@/shared/services/http.service';
import CustomStore from 'devextreme/data/custom_store';

export function ProvidersStore() {
  const store = {} as Record<string, ProviderSettingExtended>;

  return new CustomStore({
    key: nameof<ProviderSettingExtended>('providerId'),
    load: async (): Promise<ProviderSettingExtended[]> => {
      const { data } = await client.GET('/api/settings/provider');

      const result = data || [];

      for (const item of result) {
        const key = item.providerId;

        store[key] = item;
      }

      return result;
    },
    insert: async (item: ProviderSettingRequest) => {
      await client.POST('/api/settings/provider', { body: item });
      return item;
    },
    update: async (key, values) => {
      const item = Object.assign({}, store[key], values);
      await client.PUT('/api/settings/provider', { body: item });

    },
    remove: async (key) => {
      await client.DELETE('/api/settings/provider/{providerId}', {
        params: {
          path: {
            providerId: key
          }
        }
      });
    }
  });
}