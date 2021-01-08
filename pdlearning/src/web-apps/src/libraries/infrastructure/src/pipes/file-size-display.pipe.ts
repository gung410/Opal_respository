import { Injectable, Pipe, PipeTransform } from '@angular/core';

import { GlobalTranslatorService } from '../translation/global-translator.service';

@Injectable()
@Pipe({ name: 'fileSizeDisplay', pure: false })
export class FileSizeDisplayPipe implements PipeTransform {
  constructor(protected globalTranslator: GlobalTranslatorService) {}

  public transform(value: number, unitDisplay: boolean = true): string {
    const i = value === 0 ? 0 : Math.floor(Math.log(value) / Math.log(1024));
    return Number((value / Math.pow(1024, i)).toFixed(2)) * 1 + ' ' + (unitDisplay ? ['B', 'KB', 'MB', 'GB', 'TB'][i] : '');
  }
}
