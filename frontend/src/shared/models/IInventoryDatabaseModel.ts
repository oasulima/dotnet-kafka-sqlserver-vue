import type { CreatingType, InternalInventoryItem } from '@/lib/api/v1';

export interface IInventoryDatabaseModel {
  Id: string;
  FormattedLocalDate: string;
  FormattedLocalTime: string;
  UTCTicks: number;
  Symbol: string;
  Source: string;
  Qty: number;
  Filled: number;
  Avl: number;
  Rate: number;
  CreatingType: CreatingType;
  Item: InternalInventoryItem;
  HistoryItem: boolean;
  IsCoveringItem: boolean;
  CoveredItemId: string | null;
  CoveringItems?: IInventoryDatabaseModel[];
  updating?: boolean;
}