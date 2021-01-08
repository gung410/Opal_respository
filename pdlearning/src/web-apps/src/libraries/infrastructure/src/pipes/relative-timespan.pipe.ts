import { Inject, Injectable, LOCALE_ID, Pipe, PipeTransform } from '@angular/core';

import { DatePipe } from '@angular/common';
import { DateUtils } from '../utils/date.utils';
import { GlobalTranslatorService } from '../translation/global-translator.service';

@Injectable()
@Pipe({ name: 'relativeTimespan', pure: false })
export class RelativeTimespanPipe implements PipeTransform {
  constructor(@Inject(LOCALE_ID) private locale: string, protected globalTranslator: GlobalTranslatorService) {}

  public transform(value: Date, format: string): string {
    if (DateUtils.calculateAmountOfDayToPresent(value) >= 1) {
      return new DatePipe(this.locale).transform(value, format);
    }
    return DateUtils.calculateRelativeTimespan(new Date(value), this.globalTranslator);
  }
}
