import { GlobalTranslatorService } from '../translation/global-translator.service';
export class DateUtils {
  public static calculateRelativeTimespan(date: Date, translator: GlobalTranslatorService): string {
    const diff: number = Date.now() - new Date(date).getTime();
    if (diff < 0) {
      console.warn('Something went wrong with local time');
      return translator.translate('just now');
    }

    const days: number = Math.floor(diff / (60 * 60 * 24 * 1000));
    if (days > 0) {
      return translator.translate(`##days## ${days > 1 ? 'days' : 'day'} ago`, { days });
    }

    const hours: number = Math.floor(diff / (60 * 60 * 1000)) - days * 24;
    if (hours > 0) {
      return translator.translate(`##hours## ${hours > 1 ? 'hours' : 'hour'} ago`, { hours });
    }

    const minutes: number = Math.floor(diff / (60 * 1000)) - (days * 24 * 60 + hours * 60);
    if (minutes > 0) {
      return translator.translate(`##minutes## ${minutes > 1 ? 'minutes' : 'minute'} ago`, { minutes });
    }

    return translator.translate('just now');
  }

  public static calculateAmountOfDayToPresent(value: Date): number {
    const diff: number = Date.now() - new Date(value).getTime();
    return Math.floor(diff / (60 * 60 * 24 * 1000));
  }

  public static calculateAmountOfDayFromPresentToDate(value: Date): number {
    const diff: number = new Date(value).getTime() - Date.now();
    return Math.floor(diff / (60 * 60 * 24 * 1000));
  }

  public static addYear(date: Date, year: number): Date {
    date.setFullYear(date.getFullYear() + year);
    return new Date(date);
  }

  public static calcDurationInfo(miliseconds: number): { days: number; hours: number; minutes: number; seconds: number } {
    const secondsForOneDay = 60 * 60 * 24;
    const secondsForOneHour = 60 * 60;
    const secondsForOneMinute = 60;

    const totalSecondDiff = Math.round(miliseconds / 1000);
    const days = miliseconds <= 0 ? 0 : Math.floor(totalSecondDiff / secondsForOneDay);

    const totalHoursSecondDiff = totalSecondDiff - days * secondsForOneDay;
    const hours = miliseconds <= 0 ? 0 : Math.floor(totalHoursSecondDiff / secondsForOneHour);

    const totalMinutesSecondDiff = totalHoursSecondDiff - hours * secondsForOneHour;
    const minutes = miliseconds <= 0 ? 0 : Math.floor(totalMinutesSecondDiff / secondsForOneMinute);

    const seconds = miliseconds <= 0 ? 0 : totalMinutesSecondDiff - minutes * secondsForOneMinute;
    return { days: days, hours: hours, minutes: minutes, seconds: seconds };
  }

  public static removeTime(date: Date | null): Date {
    if (date == null) {
      date = new Date();
    }
    return new Date(new Date(date).toDateString());
  }

  public static compareDate(firstDate: Date, secondDate: Date, includeTime: boolean = true): number {
    const toCompareFirstDate = includeTime ? firstDate : DateUtils.removeTime(firstDate);
    const toCompareSecondDate = includeTime ? secondDate : DateUtils.removeTime(secondDate);
    if (toCompareFirstDate.getTime() === toCompareSecondDate.getTime()) {
      return 0;
    }
    if (toCompareFirstDate.getTime() > toCompareSecondDate.getTime()) {
      return 1;
    }
    return -1;
  }

  public static compareOnlyDay(firstDate: Date, secondDate: Date): number {
    const toCompareFirstDate = DateUtils.setTimeToStartOfTheDay(firstDate).getTime();
    const toCompareSecondDate = DateUtils.setTimeToStartOfTheDay(secondDate).getTime();
    if (toCompareFirstDate === toCompareSecondDate) {
      return 0;
    }
    if (toCompareFirstDate > toCompareSecondDate) {
      return 1;
    }
    return -1;
  }

  /**
   * Compare two dates without times and return diff in days.
   * @param startDate first date
   * @param endDate second date
   * @param floor round down or not
   */
  public static countDay(startDate: Date, endDate: Date, floor: boolean = true): number {
    const firstDate = startDate ? new Date(startDate) : new Date();
    const secondDate = endDate ? new Date(endDate) : new Date();
    firstDate.setHours(0, 0, 0, 0);
    secondDate.setHours(0, 0, 0, 0);
    const diffDays: number = (secondDate.getTime() - firstDate.getTime()) / (1000 * 3600 * 24);
    return floor ? Math.floor(diffDays) : diffDays;
  }

  public static compareOnlyDate(firstDateTime: Date, secondDateTime: Date): number {
    const firstDate = new Date(firstDateTime).setHours(0, 0, 0, 0);
    const secondDate = new Date(secondDateTime).setHours(0, 0, 0, 0);
    if (firstDate > secondDate) {
      return 1;
    } else if (firstDate < secondDate) {
      return -1;
    }
    return 0;
  }

  public static compareTimeOfDate(firstDate: Date, secondDate: Date): number {
    if (secondDate && firstDate) {
      const secondHour = secondDate.getHours();
      const firstHour = firstDate.getHours();
      if (secondHour < firstHour) {
        return 1;
      } else if (secondHour === firstHour && secondDate.getMinutes() < firstDate.getMinutes()) {
        return 1;
      } else if (secondHour === firstHour && secondDate.getMinutes() === firstDate.getMinutes()) {
        return 0;
      }
    }
    return -1;
  }

  public static isDateInRange(start: Date, end: Date, date: Date, includeTime: boolean = true): boolean {
    return DateUtils.compareDate(start, date, includeTime) <= 0 && DateUtils.compareDate(date, end, includeTime) <= 0;
  }

  public static setTimeToEndInDay(date: Date | null): Date | null {
    if (date == null) {
      return null;
    }
    date.setHours(23, 59, 59);
    return date;
  }

  public static setTimeToStartOfTheDay(date: Date | null): Date | null {
    if (date == null) {
      return null;
    }
    date.setHours(0, 0, 0, 0);
    return date;
  }

  public static setTimeToEndInMinute(date: Date | null): Date | null {
    if (date == null) {
      return null;
    }
    date.setHours(date.getHours(), date.getMinutes(), 59);
    return date;
  }

  public static addMinutes(date: Date, minutes: number): Date {
    return new Date(date.getTime() + minutes * 60000);
  }

  public static addDays(date: Date, days: number): Date {
    return new Date(date.getTime() + days * 24 * 60 * 60 * 1000);
  }

  public static addMonths(date: Date, months: number): Date {
    return new Date(date.setMonth(date.getMonth() + months));
  }

  public static buildDateTime(date: Date, time?: Date): Date {
    if (time) {
      return new Date(date.getFullYear(), date.getMonth(), date.getDate(), time.getHours(), time.getMinutes(), time.getSeconds(), 0);
    } else {
      return DateUtils.removeTime(date);
    }
  }

  public static getNow(): Date {
    return new Date();
  }

  public static startOfDay(): Date {
    return new Date(this.getNow().getFullYear(), this.getNow().getMonth(), this.getNow().getDate(), 0, 0, 0);
  }

  public static endOfDay(): Date {
    return new Date(this.getNow().getFullYear(), this.getNow().getMonth(), this.getNow().getDate(), 23, 59, 59);
  }

  public static startOfYear(year?: number): Date {
    if (year == null) {
      return new Date(this.getNow().getFullYear(), 0, 1, 0, 0, 1, 0);
    }
    return new Date(year, 0, 1, 0, 0, 1, 0);
  }

  public static endOfYear(year?: number): Date {
    if (year == null) {
      return new Date(this.getNow().getFullYear(), 11, 31, 23, 59, 59);
    }
    return new Date(year, 11, 31, 23, 59, 59);
  }

  public static startOfYearDateOnly(year?: number): Date {
    if (year == null) {
      return new Date(new Date(this.getNow().getFullYear(), 0, 1, 0, 0, 1, 0).toDateString());
    }
    return new Date(new Date(year, 0, 1, 0, 0, 1, 0).toDateString());
  }

  public static setDateOnly(fromDate: Date, toDate: Date): void {
    if (fromDate == null) {
      toDate = null;
      return;
    }
    toDate.setDate(fromDate.getDate());
    toDate.setMonth(fromDate.getMonth());
    toDate.setFullYear(fromDate.getFullYear());
  }

  public static setTimeOnly(fromDate: Date, toDate: Date): void {
    if (fromDate == null) {
      toDate = null;
      return;
    }
    toDate.setHours(fromDate.getHours());
    toDate.setMinutes(fromDate.getMinutes());
    toDate.setSeconds(fromDate.getSeconds());
    toDate.setMilliseconds(fromDate.getMilliseconds());
  }

  public static endOfMonth(date: Date): Date {
    return new Date(date.getFullYear(), date.getMonth() + 1, 0, 23, 59, 59);
  }

  public static startOfMonth(date: Date): Date {
    return new Date(date.getFullYear(), date.getMonth(), 1, 0, 0, 0);
  }

  public static getMondayOfWeek(date: Date): Date {
    const lessDays = date.getDay() === 0 ? 6 : date.getDay() - 1;
    return new Date(new Date(date).setDate(date.getDate() - lessDays));
  }

  public static getSundayOfWeek(date: Date): Date {
    const moreDays = date.getDay() === 0 ? 0 : 7 - date.getDay();
    return new Date(new Date(date).setDate(date.getDate() + moreDays));
  }
}
