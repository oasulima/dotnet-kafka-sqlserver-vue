import type { LocatesReportData } from '@/lib/api/v1';

export interface ILocatesReportGridModel extends LocatesReportData {
  formattedSources: string[];
}