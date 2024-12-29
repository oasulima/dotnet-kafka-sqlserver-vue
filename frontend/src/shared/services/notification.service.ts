import { hubConnectionService } from '@/shared/services/hub-connection';
import { Subject } from 'rxjs';
import { localStorageService } from './local-storage.service';
import { TimeEnum } from '@/constants';
import { groupBy, maxBy, minBy, sortBy } from 'lodash-es';

const expirationCheckIntervalInMilliseconds = TimeEnum._1hour;
const expirationTimeInMilliseconds = TimeEnum._24hours;
const notificationServiceItemsKey = 'NotificationService_Items';

interface NotificationCacheOptions {
  expirationTimeInMilliseconds: number;
  expirationCheckIntervalInMilliseconds: number;
  onItemsExpired: () => void;
}

class NotificationService {
  private updates$ = new Subject<void>();

  private cache = new NotificationCache({
    expirationTimeInMilliseconds,
    expirationCheckIntervalInMilliseconds,
    onItemsExpired: () => this.updates$.next()
  });

  constructor() {
    hubConnectionService.addListner('notification', (x) => this.hanndleNotification(x));
  }

  public getAll() {
    return this.cache.getAll();
  }

  public getUpdateStream() {
    return this.updates$.asObservable();
  }

  public markAllViewed() {
    this.cache.markAllViewed();
  }

  private hanndleNotification(notifications: GroupedNotificationDto[]): void {
    this.cache.push(notifications.map((x) => ({ ...x, isViewed: false })));
    this.updates$.next();
  }
}

class NotificationCache {
  private items: GroupedNotification[] = [];

  constructor(private options: NotificationCacheOptions) {
    this.loadAll();
    setTimeout(() => this.cleanup());
    setInterval(() => this.cleanup(), this.options.expirationCheckIntervalInMilliseconds);
  }

  public markAllViewed() {
    this.items = this.items.map((x) => ({ ...x, isViewed: true }));
    this.saveAll();
  }

  public getAll() {
    return this.items as ReadonlyArray<GroupedNotification>;
  }

  public push(x: GroupedNotification[]) {
    this.items = regroupNotifications(this.items.concat(x));
    this.saveAll();
  }

  public cleanup() {
    const oldLenght = this.items.length;
    const expirationBarrier = Date.now() - this.options.expirationTimeInMilliseconds;
    this.items = this.items.filter((x) => new Date(x.lastTime).getTime() < expirationBarrier);
    const newLenght = this.items.length;
    this.saveAll();

    if (newLenght < oldLenght && this.options.onItemsExpired) {
      this.options.onItemsExpired();
    }
  }

  private loadAll() {
    this.items = localStorageService.get(notificationServiceItemsKey) || [];
  }

  private saveAll() {
    localStorageService.set(notificationServiceItemsKey, this.items);
  }
}

function regroupNotifications(items: GroupedNotification[]) {
  const groups = groupBy(items, (x) => `${x.type}__${x.kind}__${x.groupParameters}`);
  const result = [] as GroupedNotification[];

  for (const key of Object.keys(groups)) {
    const group = groups[key];
    const lastItem = maxBy(group, (x) => x.lastTime) ?? group[0];
    const count = group.map((x) => x.count).reduce((a, b) => a + b, 0);
    const isViewed = !group.some((x) => !x.isViewed);
    const { firstTime } = minBy(group, (x) => x.firstTime) ?? group[0];

    result.push({
      ...lastItem,
      firstTime,
      count,
      isViewed
    });
  }

  return sortBy(result, [(x) => x.isViewed, (x) => -new Date(x.lastTime).getTime()]);
}

interface GroupedNotificationDto {
  type: NotificationType;
  kind: string;
  groupParameters: string;
  lastMessage: string;
  firstTime: string;
  lastTime: string;
  count: number;
}

export interface GroupedNotification extends GroupedNotificationDto {
  isViewed: boolean;
}

export enum NotificationType {
  Warning = 'Warning',
  Error = 'Error',
  Critical = 'Critical'
}

export const notificationService = new NotificationService();