import { client } from '../http.service';
import type { AddInternalInventoryItemRequest, CreatingType, InternalInventoryItem, State } from '@/lib/api/v1';
import { toResult, type Result } from '@/constants';

export class InternalInventoryService {


  public async add(model: AddInternalInventoryItemRequest): Promise<void> {
    await client.POST('/api/internal-inventory',
      {
        body: model
      }
    );
  }

  public async get(params: { symbol: string }): Promise<Result<InternalInventoryItem[]>> {
    const { data } = await client.GET('/api/internal-inventory',
      {
        params: {
          query: {
            symbol: encodeURIComponent(params.symbol)
          }
        }
      }
    );
    return toResult(data);
  }

  public async getHistory(
    params: { symbol: string }
  ): Promise<Result<InternalInventoryItem[]>> {
    const { data } = await client.GET('/api/internal-inventory/items/history',
      {
        params: {
          query: {
            symbol: encodeURIComponent(params.symbol)
          }
        }
      }
    );
    return toResult(data);
  }

  public async getItems(params: {
    symbol?: string;
    creatingType?: CreatingType;
    status?: State;
  }): Promise<Result<InternalInventoryItem[]>> {
    const { data } = await client.GET('/api/internal-inventory/items',
      {
        params: {
          query: {
            symbol: encodeURIComponent(params.symbol ?? ''),
            creatingType: params.creatingType,
            status: params.status
          }
        }
      }
    );
    return toResult(data);
  }

  public async update(model: InternalInventoryItem): Promise<void> {
    await client.PUT('/api/internal-inventory',
      {
        body: model
      }
    );
  }

  public async deactivate(model: InternalInventoryItem): Promise<void> {
    await client.PUT('/api/internal-inventory/deactivate',
      {
        body: model
      }
    );
  }

  public async activate(model: InternalInventoryItem): Promise<void> {
    await client.PUT('/api/internal-inventory/activate',
      {
        body: model
      }
    );
  }

  public async delete(model: InternalInventoryItem): Promise<void> {
    await client.PUT('/api/internal-inventory/delete',
      {
        body: model
      }
    );
  }
}

export const internalInventoryService = new InternalInventoryService();