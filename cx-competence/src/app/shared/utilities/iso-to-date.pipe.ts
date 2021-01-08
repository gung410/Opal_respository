import { Pipe, PipeTransform } from '@angular/core';
import { SurveyDateTimeUtil } from '@conexus/cx-angular-common';
import { Constant } from '../app.constant';
@Pipe({ name: 'isoToSurveyDate' })
export class IsoToSurveyDatePipe implements PipeTransform {
  transform(value: string): string {
    if (!value) {
      return '';
    }
    const result = SurveyDateTimeUtil.toSurveyFormat(
      value,
      Constant.SURVEY_DATE_FORMAT
    );

    return result === Constant.INVALID_DATE_STRING ? value : result;
  }
}
