import type { LocateModel } from '@/lib/api/v1';

export interface ILocateGridModel extends Omit<LocateModel, 'source'> {
  formattedLocalTime: string;
  utcTicks: number;
  fee: number;
  cost: number;
  pnl: number;
  formattedSources: string[];
}