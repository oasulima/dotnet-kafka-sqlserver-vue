import { capitalizeFirstLetter } from '@/@shared/utils/string.utils';

export interface ReportTemplateColumn {
  columnId: string;
  label?: string;
  inFilters?: string[];
  rangeFilters?: { from: string; to: string; }[];
  visible: boolean;
}

export interface ReportTemplate {
  id: number;
  name: string;
  saveToBlob: boolean;
  columns: Array<ReportTemplateColumn>;
  emails?: string[];
  active: boolean;
}

export const LocatesColumnCaptions = {
  date: 'Date',
  time: 'Time',
  accountId: 'Acct',
  symbol: 'Symbol',
  reqQty: 'QtyReq',
  fillQty: 'QtyOff',
  price: 'FPS',
  discountedPrice: 'CPS',
  fee: 'Fee',
  discountedFee: 'Cost',
  profit: 'PNL',
  status: 'Status',
  source: 'Source',
  provider: 'Provider'
};

export const LocatesReportColumnCaptions = Object.keys(LocatesColumnCaptions).map((x) => ({
  columnId: capitalizeFirstLetter(x),
  label: (LocatesColumnCaptions as any)[x],
  visible: true
}));