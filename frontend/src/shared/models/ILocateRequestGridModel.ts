import type { ILocateRequestModel } from './signalr/ILocateRequestModel';

export interface ILocateRequestGridModel extends ILocateRequestModel {
  formattedLocalTime: string;
  utcTicks: number;
  formattedSources: string[];
}