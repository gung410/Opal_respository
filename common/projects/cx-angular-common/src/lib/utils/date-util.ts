import * as moment_ from 'moment';
import { CxStringUtil } from './string.util';

const moment = moment_;

export class CxDateUtil {
  public static format(
    value: string | number | Date,
    format: string = 'DD/MM/YYYY'
  ): string {
    return CxDateUtil.moment(value).format(format);
  }

  public static parseDate(value: string | number | Date): Date {
    if (value instanceof Date) {
      return value;
    }
    const result = CxDateUtil.moment(value);
    if (!result.isValid()) {
      throw new Error('Can\'t parse value to Date');
    }
    return result.toDate();
  }

  public static isValidDate(value: string | number | Date): boolean {
    if (value instanceof Date) {
      return true;
    }
    if (value === undefined || value === '') {
      return false;
    }
    if (CxStringUtil.isNumber(value)) {
      return false;
    }
    if (typeof value === 'string' && value.match(/^[A-Za-z]+/) !== undefined) {
      return false;
    }
    return CxDateUtil.moment(value).isValid();
  }

  public static addMonth(
    value: string | number | Date,
    monthValue: number
  ): Date {
    return CxDateUtil.moment(value)
      .add(monthValue, 'month')
      .toDate();
  }

  public static moment(value: string | number | Date): moment_.Moment {
    if (typeof value === 'string' && CxStringUtil.isNumber(value)) {
      value = new Date(parseInt(value, 0));
    }
    return moment(value);
  }

  public static getDate(year: number, month: number, day: number) {
    return new Date(year, month - 1, day);
  }

  public static getDayStartEndOfDay(value: string | number | Date) {
    return CxDateUtil._getStartEndOfDay(value, 'day');
  }

  public static getWeekStartEndOfDay(value: string | number | Date) {
    return CxDateUtil._getStartEndOfDay(value, 'isoWeek');
  }

  public static getMonthStartEndOfDay(value: string | number | Date) {
    return CxDateUtil._getStartEndOfDay(value, 'month');
  }

  public static getQuarterStartEndOfDay(value: string | number | Date) {
    return CxDateUtil._getStartEndOfDay(value, 'quarter');
  }

  public static getYearStartEndOfDay(value: string | number | Date) {
    return CxDateUtil._getStartEndOfDay(value, 'year');
  }

  public static diffInSeconds(value1: Date, value2: Date) {
    return (value1.getTime() - value2.getTime()) / 1000;
  }

  public static diff(value1: Date, value2: Date) {
    return value1.getTime() - value2.getTime();
  }

  private static _getStartEndOfDay(
    value: string | number | Date,
    period: moment_.unitOfTime.StartOf
  ) {
    const momentDay = CxDateUtil.moment(value);
    const startDate = momentDay
      .startOf(period)
      .startOf('day')
      .toDate();
    const endDate = momentDay
      .endOf(period)
      .endOf('day')
      .toDate();
    return {
      start: startDate,
      end: endDate
    };
  }
}
