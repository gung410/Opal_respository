import * as moment_ from 'moment';
const moment = moment_;

export class SurveyDateTimeUtil {
  static surveyToServerFormat(date: string, format: string): string {
    if (date) {
      return moment.utc(date, format).toDate().toISOString();
    }

    return '';
  }

  static toSurveyFormat(date: string, format: string): string {
    if (date) {
      return moment(date).format(format);
    }

    return '';
  }

  static compareDate(date1: Date, date2: Date, format: string , withoutTime: boolean): boolean {
    return withoutTime ?
           moment(date1, format).isSameOrAfter(moment(date2, format)) :
           moment(date1).isSameOrAfter(date2);
  }
}
