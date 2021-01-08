import { Injectable, Pipe, PipeTransform } from '@angular/core';

import { GlobalTranslatorService } from '../translation/global-translator.service';
import { TimeUtils } from '../utils/time.utils';

@Injectable()
@Pipe({ name: 'durationInHAndM', pure: false })
export class DurationInHAndM implements PipeTransform {
  constructor(protected globalTranslator: GlobalTranslatorService) {}

  public transform(value: number): string {
    return TimeUtils.displayDurationInHAndM(value);
  }
}
