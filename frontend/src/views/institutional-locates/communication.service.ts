import { uiDiplayStateService } from '@/@shared/services/ui-display-state.service';
import { DateFormats, nameof, TimeEnum } from '@/constants';
import type { InternalInventoryItem, LocateModel, LocateRequestModel, QuoteResponseStatusEnum } from '@/lib/api/v1';
import { formatQuoteSourceInfos } from '@/shared/formatters/quote-source-infos.formatter';
import { toInventoryDatabaseModel } from '@/shared/mappers/inventory-database-model.mapper';
import type { IInventoryDatabaseModel } from '@/shared/models/IInventoryDatabaseModel';
import type { ILocateGridModel } from '@/shared/models/ILocateGridModel';
import type { ILocateRequestGridModel } from '@/shared/models/ILocateRequestGridModel';
import type { IMostLocatedModel } from '@/shared/models/IMostLocatedModel';
import { internalInventoryService } from '@/shared/services/api/internal-inventory-service';
import { hubConnectionService } from '@/shared/services/hub-connection';
import dayjs from 'dayjs';
import duration from 'dayjs/plugin/duration';
import utc from 'dayjs/plugin/utc';
import ArrayStore from 'devextreme/data/array_store';
import { uniq } from 'lodash-es';
import { combineLatest, debounceTime } from 'rxjs';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Subject } from 'rxjs/internal/Subject';

dayjs.extend(utc);
dayjs.extend(duration);

interface InstitutionalLocatesServiceState {
  symbolFilter: string;
  isInventoryDatabaseLinked: boolean;
  isMostLocatedLinked: boolean;
  isLocatesLinked: boolean;
  isLocateRequestsLinked: boolean;
}

const defaultState: InstitutionalLocatesServiceState = {
  symbolFilter: '',
  isInventoryDatabaseLinked: false,
  isMostLocatedLinked: false,
  isLocatesLinked: false,
  isLocateRequestsLinked: false
};

const STATE_KEY = 'InstitutionalLocatesService__state';

export class InstitutionalLocatesService {

  private dayDataCleaned: Subject<void> = new Subject<void>();
  public $dayDataCleaned = this.dayDataCleaned.asObservable();
  public readonly $symbolFilter = new BehaviorSubject<string>('');
  public readonly $isInventoryDatabaseLinked = new BehaviorSubject(false);
  public readonly $isMostLocatedLinked = new BehaviorSubject(false);
  public readonly $isLocatesLinked = new BehaviorSubject(false);
  public readonly $isLocateRequestsLinked = new BehaviorSubject(false);

  constructor() {
    this.runSignalR();
    this.runDayDataCleaner();
    this.initState();
  }

  private initState() {
    const rawState = uiDiplayStateService.loadProp<InstitutionalLocatesServiceState>(STATE_KEY);
    const state = Object.assign({}, defaultState, rawState);
    this.applyState(state);

    combineLatest([
      this.$symbolFilter,
      this.$isInventoryDatabaseLinked,
      this.$isMostLocatedLinked,
      this.$isLocatesLinked,
      this.$isLocateRequestsLinked
    ])
      .pipe(debounceTime(1000))
      .subscribe(() => {
        const newState = this.extractState();

        uiDiplayStateService.saveProp(STATE_KEY, newState);
      });
  }

  private extractState(): InstitutionalLocatesServiceState {
    return {
      symbolFilter: this.$symbolFilter.value,
      isInventoryDatabaseLinked: this.$isInventoryDatabaseLinked.value,
      isMostLocatedLinked: this.$isMostLocatedLinked.value,
      isLocatesLinked: this.$isLocatesLinked.value,
      isLocateRequestsLinked: this.$isLocateRequestsLinked.value
    };
  }

  private applyState(state: InstitutionalLocatesServiceState) {
    this.$symbolFilter.next(state.symbolFilter);
    this.$isInventoryDatabaseLinked.next(state.isInventoryDatabaseLinked);
    this.$isMostLocatedLinked.next(state.isMostLocatedLinked);
    this.$isLocatesLinked.next(state.isLocatesLinked);
    this.$isLocateRequestsLinked.next(state.isLocateRequestsLinked);
  }

  private runDayDataCleaner() {
    setTimeout(() => {
      this.clearData();
      setInterval(() => {
        this.clearData();
      }, TimeEnum._24hours);
    }, this.getDataCleanerDelayMills());
  }

  private clearData() {
    this.locateRequests.clear();
    this.locateRequestsStore.clear();
    this.locateRequestsStore.push([{ type: 'remove' }]); // trigger assosiated tables refresh

    this.locates.clear();
    this.locatesStore.clear();
    this.locatesStore.push([{ type: 'remove' }]); // trigger assosiated tables refresh

    this.mostLocated.clear();
    this.mostLocatedStore.clear();
    this.mostLocatedStore.push([{ type: 'remove' }]); // trigger assosiated tables refresh

    this.dayDataCleaned.next();
  }

  private getDataCleanerDelayMills() {
    const now = dayjs.utc();
    const run = now.startOf('day').add(this.getDataCleanerRunTimeUtc());
    if (run.isBefore(now)) {
      return run.add(1, 'd').diff(now);
    }
    else {
      return run.diff(now);
    }
  }

  private getDataCleanerRunTimeUtc() {

    const groups = import.meta.env.VITE_DATA_CLEANER_RUN_TIME_UTC.match(/(\d{2}):(\d{2}):(\d{2})/);

    if (groups == null) {
      throw new Error('Set VITE_DATA_CLEANER_RUN_TIME_UTC in the config');
    }

    return dayjs.duration({
      hours: parseInt(groups[1]),
      minutes: parseInt(groups[2]),
      seconds: parseInt(groups[3])
    });
  }

  private readonly locateRequests: Map<string, ILocateRequestGridModel> = new Map<string, ILocateRequestGridModel>();
  public readonly $locateRequests = new Subject<ILocateRequestGridModel>();

  public locateRequestsStore = new ArrayStore<ILocateRequestGridModel>({
    key: nameof<ILocateRequestGridModel>('id'),
    data: []
  });

  public getLocateRequests(params: { accountId: string; }) {
    return [...this.locateRequests.values()].filter((item) => item.accountId == params.accountId);
  }

  private readonly locates: Map<string, ILocateGridModel> = new Map<string, ILocateGridModel>();
  public readonly $locates = new Subject<ILocateGridModel>();

  public getLocates(params: { accountId: string; }) {
    return [...this.locates.values()].filter((item) => item.accountId == params.accountId);
  }

  public locatesStore = new ArrayStore<ILocateGridModel>({
    key: [nameof<ILocateGridModel>('quoteId'), nameof<ILocateGridModel>('status')],
    data: []
  });

  private inventoryItems: Map<string, IInventoryDatabaseModel> = new Map<string, IInventoryDatabaseModel>();

  public startDeactivating(itemId: string): boolean {
    const item = this.inventoryItems.get(itemId);
    if (item && this.isIIItemActive(item)) {
      return this._startUpdatingII(item);
    }

    return false;
  }

  public startActivating(itemId: string): boolean {
    const item = this.inventoryItems.get(itemId);
    if (item && this.isIIItemInactive(item)) {
      return this._startUpdatingII(item);
    }

    return false;
  }

  public isIIItemActive(item: IInventoryDatabaseModel) {
    return !item.HistoryItem && item.Item.status == 'Active';
  }

  public isIIItemInactive(item: IInventoryDatabaseModel) {
    return !item.HistoryItem && item.Item.status == 'Inactive';
  }

  public startUpdatingII(itemId: string) {
    const item = this.inventoryItems.get(itemId);
    return this._startUpdatingII(item);
  }

  private _startUpdatingII(item?: IInventoryDatabaseModel): boolean {
    if (item && !item.updating) {
      item.updating = true;
      this.inventoryItems.set(item.Id, item);
      this.inventoryDatabaseStore.push([{ type: 'update', data: { updating: true }, key: item.Id }]);
      return true;
    }
    return false;
  }

  private addInternalInventoryItem(item: InternalInventoryItem, historyItem: boolean) {
    if (this.$symbolFilter.value != item.symbol || !this.$isInventoryDatabaseLinked.value) return;

    if (!this.inventoryItems.has(item.id) || this.inventoryItems.get(item.id)!.Item.version < item.version) {
      const gridItem = toInventoryDatabaseModel(item, historyItem);
      if (this.inventoryItems.has(item.id)) {
        this.inventoryDatabaseStore.push([{ type: 'update', data: gridItem, key: gridItem.Id }]);
      }
      else {
        this.inventoryDatabaseStore.push([{ type: 'insert', data: gridItem }]);
      }
      this.inventoryItems.set(gridItem.Id, gridItem);
    }
  }
  public inventoryDatabaseStore = new ArrayStore<IInventoryDatabaseModel, string>({
    key: nameof<IInventoryDatabaseModel>('Id'),
    data: []
  });
  public reloadInventoryGrid(params: { isLinked: boolean, symbol: string }) {
    const operations = Array.from(this.inventoryItems.keys())
      .map((x) => ({ type: 'remove', key: x } as { type: 'update' | 'insert' | 'remove', key: string }));
    this.inventoryDatabaseStore.push(operations);
    this.inventoryItems.clear();

    if (params.symbol && params.isLinked) {
      internalInventoryService.get({ symbol: params.symbol }).then((data) => {
        if (data.type == 'error') return;

        for (const item of data.data) {
          this.addInternalInventoryItem(item, false);
        }
      }, () => { });
      internalInventoryService.getHistory({ symbol: params.symbol }).then((data) => {
        if (data.type == 'error') return;

        for (const item of data.data) {
          this.addInternalInventoryItem(item, true);
        }
      }, () => { });
    }
    return [];
  }

  public locatesProviderIdsArr = [] as any[];
  public locatesProviderIdsSet = new Set<string>();

  private mostLocated: Map<string, IMostLocatedModel> = new Map<string, IMostLocatedModel>();

  public mostLocatedStore = new ArrayStore<IMostLocatedModel>({
    key: nameof<IMostLocatedModel>('Symbol'),
    data: []
  });

  public updateLocatesProviderIds() {
    const filters = this.buildProviderIdFilters([...this.locatesProviderIdsSet]);
    this.locatesProviderIdsArr.splice(0);
    this.locatesProviderIdsArr.push(...filters);
  }

  private buildProviderIdFilters(providerIds: string[]) {
    const filterItems = providerIds.map((providerId) => ({
      text: providerId,
      value: ['source', 'contains', providerId]
    }));

    return [
      ...filterItems
    ];
  }

  private handleLocateRequest(locateRequest: LocateRequestModel) {
    if (this.locateRequests.has(locateRequest.id)) {
      return;
    }
    this.insertIntoLocateRequestStore(locateRequest);
    this.updateMostLocatedWithLocateRequest(locateRequest);
  }

  private getLocateMapKey(locate: LocateModel) {
    return locate.quoteId + locate.status;
  }

  private handleLocate(locate: LocateModel) {
    if (this.locates.has(this.getLocateMapKey(locate))) {
      return;
    }

    this.insertIntoLocateStore(locate);
    this.updateMostLocatedWithLocate(locate);
  }

  private runSignalR() {
    hubConnectionService.addListner('locate-request', (locateRequest: LocateRequestModel) =>
      this.handleLocateRequest(locateRequest)
    );

    hubConnectionService.addListner('locate-request-history', (locateRequests: LocateRequestModel[]) => {
      locateRequests.forEach((locateRequest) => {
        this.handleLocateRequest(locateRequest);
      });
    });

    hubConnectionService.addListner('locate', (locate: LocateModel) => {
      this.handleLocate(locate);
    });

    hubConnectionService.addListner('locate-history', (locates: LocateModel[]) => {
      locates.forEach((locate) => {
        this.handleLocate(locate);
      });
    });

    hubConnectionService.addListner('internal-inventory', (item: InternalInventoryItem) => {
      this.addInternalInventoryItem(item, false);
    });
  }

  private insertIntoLocateRequestStore(locateRequest: LocateRequestModel): void {
    const time = dayjs.utc(locateRequest.time);

    const gridLocateRequest: ILocateRequestGridModel = {
      ...locateRequest,
      formattedLocalTime: time.local().format(DateFormats.DAYJS_TIME),
      utcTicks: time.valueOf(),
      formattedSources: formatQuoteSourceInfos(locateRequest.sourceDetails)
    };

    this.locateRequestsStore.push([{ type: 'insert', data: gridLocateRequest }]);

    this.locateRequests.set(gridLocateRequest.id, gridLocateRequest);
    this.$locateRequests.next(gridLocateRequest);
  }

  private isZeroPNL(locateStatus: QuoteResponseStatusEnum) {
    return 'Filled' != locateStatus && 'Partial' != locateStatus;
  }

  private insertIntoLocateStore(locate: LocateModel): void {
    const time = dayjs.utc(locate.time);
    const fee = locate.qtyFill * locate.price;
    const cost = locate.qtyFill * locate.discountedPrice;
    const pnl = this.isZeroPNL(locate.status) ? 0 : fee - cost;
    const sources = uniq(locate.sourceDetails.map((x) => x.source));

    const gridLocate: ILocateGridModel = {
      ...locate,
      formattedLocalTime: time.local().format(DateFormats.DAYJS_TIME),
      utcTicks: time.valueOf(),
      fee,
      cost,
      pnl,
      formattedSources: formatQuoteSourceInfos(locate.sourceDetails)
    };

    sources.forEach((x) => this.locatesProviderIdsSet.add(x));
    this.updateLocatesProviderIds();

    this.locatesStore.push([{ type: 'insert', data: gridLocate }]);

    this.locates.set(this.getLocateMapKey(locate), gridLocate);
    this.$locates.next(gridLocate);
  }

  private updateMostLocatedWithLocateRequest(locateRequest: LocateRequestModel): void {
    const symbol = locateRequest.symbol;
    const requestsQty = locateRequest.qtyReq;
    const mostLocated = this.mostLocated.get(symbol);

    if (mostLocated) {
      mostLocated.Requests += 1;
      mostLocated.ReqQty += requestsQty;

      this.mostLocatedStore.push([{
        type: 'update', data: {
          Requests: mostLocated.Requests,
          ReqQty: mostLocated.ReqQty
        }, key: mostLocated.Symbol
      }]);
    }
    else {
      const newMostLocated: IMostLocatedModel = {
        Symbol: symbol,
        Cancels: 0,
        Requests: 1,
        ReqQty: requestsQty,
        Fills: 0,
        FillQty: 0
      };

      this.mostLocated.set(symbol, newMostLocated);
      this.mostLocatedStore.push([{ type: 'insert', data: newMostLocated }]);
    }
  }

  private updateMostLocatedWithLocate(locate: LocateModel): void {
    const symbol = locate.symbol;
    const isFilledLocate = locate.status === 'Filled';
    const filledLocate = isFilledLocate ? 1 : 0;
    const canceledLocate = locate.status === 'Cancelled' ? 1 : 0;
    const filledQty = isFilledLocate ? locate.qtyFill : 0;

    const mostLocated = this.mostLocated.get(symbol);
    if (mostLocated) {

      mostLocated.Cancels += canceledLocate;
      mostLocated.Fills += filledLocate;
      mostLocated.FillQty += filledQty;

      this.mostLocatedStore.push([{
        type: 'update', data: {
          Cancels: mostLocated.Cancels,
          Fills: mostLocated.Fills,
          FillQty: mostLocated.FillQty
        }, key: mostLocated.Symbol
      }]);

    }
    else {
      const newMostLocated: IMostLocatedModel = {
        Symbol: symbol,
        Cancels: canceledLocate,
        Requests: 0,
        ReqQty: 0,
        Fills: filledLocate,
        FillQty: filledQty
      };

      this.mostLocated.set(symbol, newMostLocated);
      this.mostLocatedStore.push([{ type: 'insert', data: newMostLocated }]);
    }
  }
}

export const institutionalLocatesService = new InstitutionalLocatesService();