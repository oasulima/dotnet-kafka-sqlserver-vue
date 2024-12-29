import type { QuoteResponseStatusEnum, QuoteSourceInfo } from '@/lib/api/v1';

export interface ILocateModel {
  quoteId: string;
  accountId: string;
  time: string;
  symbol: string;
  reqQty: number;
  qtyFill: number;
  price: number;
  discountedPrice: number;
  status: QuoteResponseStatusEnum;
  errorMessage: string | null;
  source: string;
  sourceDetails: QuoteSourceInfo[];
}