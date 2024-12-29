import { DateFormats } from '@/constants';
import dayjs from 'dayjs';
import type { IInventoryDatabaseModel } from '../models/IInventoryDatabaseModel';
import type { InternalInventoryItem } from '@/lib/api/v1';


export function toInventoryDatabaseModel(item: InternalInventoryItem, historyItem: boolean): IInventoryDatabaseModel {
  const date = dayjs.utc(item.createdAt);
  const localDate = date.local();

  return {
    Id: item.id,
    FormattedLocalDate: localDate.format(DateFormats.DAYJS_DATE),
    FormattedLocalTime: localDate.format(DateFormats.DAYJS_TIME),
    UTCTicks: date.valueOf(),
    Symbol: item.symbol,
    Qty: item.status === 'Deleted' ? 0 : item.quantity + item.soldQuantity,
    Filled: item.soldQuantity,
    Avl: item.status === 'Deleted' && !item.coveredInvItemId ? -item.soldQuantity : item.quantity,
    Rate: item.price,
    Source: item.source,
    Item: item,
    CreatingType: item.creatingType,
    HistoryItem: historyItem,
    IsCoveringItem: !!item.coveredInvItemId,
    CoveredItemId: item.coveredInvItemId,
    updating: false
  };
}