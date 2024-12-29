import { match } from 'ts-pattern';

export class DateFormats {
  public static DAYJS_DATE = 'MM/DD/YYYY';
  public static DAYJS_TIME = 'HH:mm:ss';
  public static DAYJS_DATETIME = 'MM/DD/YYYY HH:mm:ss';
}


export const nameof = <T>(name: Extract<keyof T, string>): string => name;

export enum TimeEnum {
  _1second = 1_000,
  _1mins = 60_000,
  _15mins = 900_000,
  _1hour = 3_600_000,
  _24hours = 86_400_000
}

export enum SortOrderEnum {
  asc = 'asc',
  desc = 'desc'
}

export type UserRoleEnum = 'Unknown' | 'Admin' | 'Viewer'

export type Result<T> =
  | { type: 'ok'; data: T }
  | { type: 'error' };

export function toResult<T>(data: T | undefined): Result<T> {
  return match(data)
    .returnType<Result<T>>()
    .with(undefined, () => ({ type: 'error' }))
    .otherwise(() => ({ type: 'ok', data: data! }));
}