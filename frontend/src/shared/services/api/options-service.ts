import type { CreatingType, State, StringSelectValue } from '@/lib/api/v1';
import { client } from '../http.service';
import { toResult, type Result } from '@/constants';

export class OptionsService {
  public async getSources(): Promise<Result<StringSelectValue[]>> {
    const { data } = await client.GET('/api/options/sources');
    return toResult(data);
  }

  public async getCreatingTypes(): Promise<Result<CreatingType[]>> {
    const { data } = await client.GET('/api/options/creating-types');
    return toResult(data);
  }

  public async getInternalInventoryStatuses(): Promise<Result<State[]>> {
    const { data } = await client.GET('/api/options/internal-inventory/statuses');
    return toResult(data);
  }
}

export default new OptionsService();