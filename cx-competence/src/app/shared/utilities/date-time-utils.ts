import { isEmpty } from 'lodash';
import * as moment from 'moment';
import { AppConstant, Constant } from '../app.constant';

export class DateTimeUtil {
  static surveyToServerFormat(date: string): string {
    if (date) {
      return moment
        .utc(date, Constant.SURVEY_DATE_FORMAT)
        .toDate()
        .toISOString();
    }

    return Constant.STRING_EMPTY;
  }

  static surveyToDateLocalTime(date: string): Date {
    if (date) {
      return moment(date, Constant.SURVEY_DATE_FORMAT).toDate();
    }

    return new Date();
  }

  static toSurveyFormat(date: string): string {
    if (date) {
      return moment(date).format(Constant.SURVEY_DATE_FORMAT);
    }

    return Constant.STRING_EMPTY;
  }

  static compareDate(date1: Date, date2: Date, withoutTime: boolean): boolean {
    return withoutTime
      ? moment(date1, Constant.COMPARE_DATE_FORMAT).isSameOrAfter(
          moment(date2, Constant.COMPARE_DATE_FORMAT)
        )
      : moment(date1).isSameOrAfter(date2);
  }

  /**
   * Determines whether the date is in the past (either yesterday or earlier) or not.
   * e.g: date is '2019-11-16 17:00:00.0000000' and now is '2019-11-17 17:00:00.0000000', it should return True.
   * @param date The date which will used to compare with the current date.
   */
  static isInThePast(date: Date | string): boolean {
    const oneDayMilliseconds = 86400000; // 24 hours * 60 minutes * 60 seconds * 1000 ticks
    if (typeof date === 'string') {
      date = new Date(date);
    }

    return (date.getTime() - new Date().getTime()) / oneDayMilliseconds <= -1.0;
  }

  /**
   * Converts the date into date string format.
   * @param date The date object or ISO string
   */
  static toDateString(
    date: Date | string,
    format: string = AppConstant.backendDateFormat
  ): string {
    if (!(date instanceof Date) && isEmpty(date)) {
      return 'N/A';
    }

    return moment(date).format(format);
  }

  /**
   * Parse a survey date into a javascript Date object.
   * @param date The date string in format AppConstant.backendDateFormat
   */
  static parseSurveyDate(date: string): Date {
    return moment(date, AppConstant.backendDateFormat).toDate();
  }

  /**
   * Get time from a date
   * @param date date ISO string
   */
  static getTimeFromNow(date: string): string {
    return moment(date).fromNow();
  }
}
