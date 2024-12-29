export interface IExternalProviderQuoteResponse {
  symbol: string;
  quantity: number;
  price: number;
  discountedPrice: number;
  providerId: string;
  providerName: string;
  assetType: AssetType;
  dateTime: Date
}

export enum AssetType {
  Locate = 'Locate',
  PreBorrow = 'PreBorrow'
}