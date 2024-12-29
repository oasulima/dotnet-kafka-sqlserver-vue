import type { QuoteResponseStatusEnum } from '@/lib/api/v1';

export interface ILocatesReportFilter {
  accountId: string | null;
  symbol: string | null;
  status: QuoteResponseStatusEnum;
  timeFrom: Date;
  timeTo: Date;
  providerId: string | null;
}