import type { ILocateModel } from './signalr/ILocateModel';

export interface ILocateGridModel extends Omit<ILocateModel, 'source'> {
  formattedLocalTime: string;
  utcTicks: number;
  fee: number;
  cost: number;
  pnl: number;
  formattedSources: string[];
}