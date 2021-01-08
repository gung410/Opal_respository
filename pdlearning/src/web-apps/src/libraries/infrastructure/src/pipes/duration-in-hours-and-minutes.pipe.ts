import { Injectable, Pipe, PipeTransform } from '@angular/core';

import { GlobalTranslatorService } from '../translation/global-translator.service';
import { TimeUtils } from '../utils/time.utils';

@Injectable()
@Pipe({ name: 'durationInHoursAndMinutes', pure: false })
export class DurationInHoursAndMinutes implements PipeTransform {
  constructor(protected globalTranslator: GlobalTranslatorService) {}

  public transform(value: number): string {
    return TimeUtils.displayDurationInHoursAndMinutes(value, this.globalTranslator);
  }
}
