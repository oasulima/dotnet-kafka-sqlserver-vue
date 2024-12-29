import type { QuoteSourceInfo } from '@/lib/api/v1';

export interface ILocateRequestModel {
  id: string;
  accountId: string;
  time: string;
  symbol: string;
  qtyReq: number;
  qtyOffer: number;
  price: number;
  discountedPrice: number;
  source: string;
  sourceDetails: QuoteSourceInfo[];
}