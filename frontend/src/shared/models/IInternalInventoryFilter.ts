import type { CreatingType, State } from '@/lib/api/v1';

export interface IInternalInventoryFilter {
  symbol?: string;
  creatingType?: CreatingType;
  status?: State;
}