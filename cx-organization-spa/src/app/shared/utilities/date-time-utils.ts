import * as moment from 'moment';
import { AppConstant } from '../app.constant';
import {
  compareDateFormat,
  stringEmpty,
  surveyDateFormat
} from '../common.constant';

export const DAY_OF_WEEK = [
  'Sunday',
  'Monday',
  'Tuesday',
  'Wednesday',
  'Thursday',
  'Friday',
  'Saturday'
];
export class DateTimeUtil {
  static surveyToServerFormat(date: string): string {
    if (date) {
      return moment.utc(date, surveyDateFormat).toDate().toISOString();
    }

    return stringEmpty;
  }

  static getWeekDay(date: Date | string): string {
    if (typeof date === 'string') {
      date = new Date(date);
    }

    return DAY_OF_WEEK[date.getDay()];
  }

  static sortDay<T>(days: T[], sortType: 'asc' | 'desc' = 'asc'): T[] {
    return days.sort((day, comparedDay) => {
      return (
        DAY_OF_WEEK.indexOf(day.toString()) -
        DAY_OF_WEEK.indexOf(comparedDay.toString())
      );
    });
  }

  static getDateTimeFromNow(date: Date): string {
    if (date == null) {
      return;
    }

    return moment(date).fromNow();
  }

  static getOrderOfDayInMonth(date: Date | string): number {
    if (typeof date === 'string') {
      date = new Date(date);
    }

    let divisionResult = Math.floor(date.getDate() / DAY_OF_WEEK.length);

    if (date.getDate() % DAY_OF_WEEK.length > 0) {
      divisionResult++;
    }

    return divisionResult;
  }

  static getOrderOfDayInMonthText(targetDate: Date): string {
    const orderNumber = DateTimeUtil.getOrderOfDayInMonth(targetDate);

    switch (orderNumber) {
      case 1:
        return 'first';
      case 2:
        return 'second';
      case 3:
        return 'third';
      case 4:
        return 'fourth';
      default:
        return 'last';
    }
  }

  static IsDateInPast(date: Date | string): boolean {
    if (!date) {
      return;
    }

    if (typeof date === 'string') {
      date = new Date(date);
    }

    return this.compareDate(date, new Date(), true, true) < 0;
  }

  static removeSeconds(date?: Date | string): Date {
    if (!date) {
      return null;
    }

    if (typeof date === 'string') {
      date = new Date(date);
    }

    date.setSeconds(0, 0);

    return date;
  }

  /**
   *
   * @param firstDate the date compares to secondDate
   * @param secondDate the date is compared with firstDate
   * @returns 1: firstDate is greater than secondDate
   * @returns 0: Equal
   * @returns -1: firstDate is less than secondDate
   */
  static compareTimeOfDate(firstDate: Date, secondDate: Date): number {
    if (secondDate && firstDate) {
      const secondHour = secondDate.getHours();
      const firstHour = firstDate.getHours();
      if (secondHour < firstHour) {
        return 1;
      } else if (
        secondHour === firstHour &&
        secondDate.getMinutes() < firstDate.getMinutes()
      ) {
        return 1;
      } else if (
        secondHour === firstHour &&
        secondDate.getMinutes() === firstDate.getMinutes()
      ) {
        return 0;
      }
    }

    return -1;
  }

  static convertToLocaleFormat(
    date: Date,
    format: 'LT' | 'LTS' | 'LLL' | 'lll'
  ): string {
    if (date == null) {
      return;
    }

    return moment(date).format(format);
  }

  static convertTimeFormat(time: string, format: '24h' | '12h'): string {
    if (time == null) {
      return;
    }

    switch (format) {
      case '12h':
        return moment(time, 'hh:mm').format('LT');
      case '24h':
        return moment(time, ['h:mm A']).format('HH:mm');
      default:
        return moment(time, 'hh:mm').format('LT');
    }
  }

  static convertNumberToMonthName(monthIndex: number): string {
    return moment().month(monthIndex).format('MMMM');
  }

  static surveyToDateLocalTimeISO(date: string): string {
    if (date) {
      const localDateObj = this.surveyToDateLocalTime(date);
      if (localDateObj) {
        return localDateObj.toISOString();
      }
    }

    return stringEmpty;
  }

  static surveyToDateLocalTime(date: string): Date {
    if (date) {
      return moment(date, surveyDateFormat).toDate();
    }

    return;
  }

  static surveyToEndDateLocalTimeISO(date: string): string {
    if (date) {
      const localDateObj = this.surveyToEndDateLocalTime(date);
      if (localDateObj) {
        return localDateObj.toISOString();
      }
    }

    return stringEmpty;
  }

  static surveyToEndDateLocalTime(date: string): Date {
    if (date) {
      return moment(date, surveyDateFormat)
        .add('days', 1)
        .add('seconds', -1)
        .toDate();
    }

    return;
  }

  static getEndDate(date: Date): Date {
    if (date) {
      return moment(date).add('days', 1).add('seconds', -1).toDate();
    }

    return new Date();
  }

  static toSurveyFormat(date: string): string {
    if (date) {
      return moment(date).format(surveyDateFormat);
    }

    return stringEmpty;
  }

  /**
   * Converts the date into date string format.
   * @param date The date object or ISO string
   */
  static toDateString(
    date: Date | string,
    format: string = AppConstant.backendDateFormat
  ): string {
    return moment(date).format(format);
  }

  static compareDate(
    date1: Date,
    date2: Date,
    includeTime: boolean = true,
    ignoreSecond: boolean = false
  ): number {
    let d1 = new Date(date1);
    let d2 = new Date(date2);
    if (ignoreSecond) {
      d1.setSeconds(0, 0);
      d2.setSeconds(0, 0);
    }
    if (!includeTime) {
      d1 = this.removeTime(d1);
      d2 = this.removeTime(d2);
    }

    const same = d1.getTime() === d2.getTime();
    if (same) {
      return 0;
    }

    if (d1 > d2) {
      return 1;
    }

    if (d1 < d2) {
      return -1;
    }
  }

  static getLocalTimezone(): number {
    const timezoneOffsetCycle = -60;

    return new Date().getTimezoneOffset() / timezoneOffsetCycle;
  }

  static getTimeFromDate(date: Date, timeFormat?: string): string {
    if (timeFormat) {
      return moment(date).format(timeFormat);
    }

    return moment(date).format('HH:mm:ss');
  }

  static removeTime(date: Date | null): Date {
    if (date == null) {
      date = new Date();
    }

    return new Date(new Date(date).toDateString());
  }

  static buildDateTime(date: Date, time?: Date): Date {
    if (time) {
      return new Date(
        date.getFullYear(),
        date.getMonth(),
        date.getDate(),
        time.getHours(),
        time.getMinutes(),
        time.getSeconds(),
        0
      );
    } else {
      return DateTimeUtil.removeTime(date);
    }
  }

  static createDefaultDateFromTime(time: string): Date {
    return moment(`2000-01-01 ${time}`).toDate();
  }

  static updateDateWithoutTime(oldDate: Date, newDate: Date): Date {
    if (typeof oldDate === 'string') {
      oldDate = new Date(oldDate);
    }
    if (typeof newDate === 'string') {
      newDate = new Date(newDate);
    }
    oldDate.setDate(newDate.getDate());
    oldDate.setMonth(newDate.getMonth());
    oldDate.setFullYear(newDate.getFullYear());

    return oldDate;
  }

  static convertDateToComparableNumber(date: Date): number {
    if (!date) {
      return null;
    }
    if (typeof date === 'string') {
      date = new Date(date);
    }
    const yearNumber = date.getFullYear();
    const monthNumber = date.getMonth();
    const dayNumber = date.getDate();
    const result =
      yearNumber * this.MULTIPLE_FOR_YEAR +
      monthNumber * this.MULTIPLE_FOR_MONTH +
      dayNumber;

    return result;
  }

  private static readonly MULTIPLE_FOR_YEAR: number = 10000;
  private static readonly MULTIPLE_FOR_MONTH: number = 100;
}
