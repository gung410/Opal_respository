import { ChangeDetectorRef, Injectable, Pipe, PipeTransform } from '@angular/core';

import { GlobalTranslatorService } from '../global-translator.service';
import { TranslatePipe } from '@ngx-translate/core';

@Injectable()
@Pipe({ name: 'globalTranslator', pure: false })
export class GlobalTranslatorPipe extends TranslatePipe implements PipeTransform {
  constructor(protected globalTranslator: GlobalTranslatorService, protected cdr: ChangeDetectorRef) {
    super(globalTranslator, cdr);
  }
}
