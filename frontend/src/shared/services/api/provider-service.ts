import { client } from '../http.service';

export class ProviderService {
  public async activateProvider(providerId: string): Promise<void> {
    await client.POST('/api/settings/provider/{providerId}/activate', {
      params: {
        path: { providerId: providerId }
      }
    });
  }
}

export const providerService = new ProviderService();