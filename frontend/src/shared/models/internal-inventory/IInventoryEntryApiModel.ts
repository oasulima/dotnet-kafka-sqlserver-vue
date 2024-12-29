import type { CreatingType } from '@/lib/api/v1';

export interface IInventoryItem {
  id: string;
  version: number;
  symbol: string;
  quantity: number;
  soldQuantity: number;
  price: number;
  source: string;
  creatingType: CreatingType;
  coveredInvItemId?: string;

  status: InventoryItemStateEnum;
  createdAt: string;
}

export enum InventoryItemStateEnum {
  Active = 'Active',
  Inactive = 'Inactive',
  Deleted = 'Deleted'
}

