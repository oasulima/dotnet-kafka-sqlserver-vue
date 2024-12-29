// import * as moment from 'moment';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
dayjs.extend(utc);

export class DateUtils {
  private static formatDate(date: Date | string, format: string, local: boolean) {
    if (date) {
      let result = dayjs.utc(date);
      if (local) {
        result = result.local();
      }
      return result.format(format);
    }
    return '';
  }

  public static formatUtcDate(date: Date | string, format: string) {
    return this.formatDate(date, format, false);
  }

  public static formatLocalDate(date: Date | string, format: string) {
    return this.formatDate(date, format, true);
  }
}
