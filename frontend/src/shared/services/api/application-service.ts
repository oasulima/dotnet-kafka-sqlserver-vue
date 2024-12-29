import { client } from '../http.service';

class ApplicationService {
  public async getRunningEnvName(): Promise<string> {
    const { data, error } = await client.GET('/api/Application/running-env');
    return data ?? '';
  }
}

export const applicationService = new ApplicationService();