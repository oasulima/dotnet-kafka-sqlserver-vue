import type { InventoryRequest } from '@/lib/api/v1';
import { client } from '@/shared/services/http.service';

class InventoryService {
  public async get(params: InventoryRequest) {
    const { data } = await client.POST('/api/Inventory/inventory',
      { body: params }
    );

    console.log('InventoryService get: ', data);
    return Object.values(data ?? {}).flatMap((x) => x);
  }
}

export interface QuoteRequest {
  id: number;
  symbol: string;
  quantity: number;
  accountId: string;
}

export const inventoryService = new InventoryService();
