import type { LocateRequestModel } from '@/lib/api/v1';

export interface ILocateRequestGridModel extends LocateRequestModel {
  formattedLocalTime: string;
  utcTicks: number;
  formattedSources: string[];
}