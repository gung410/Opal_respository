import { ChangeDetectorRef, Injectable, Pipe, PipeTransform } from '@angular/core';

import { LocalTranslatorService } from '../local-translator.service';
import { TranslatePipe } from '@ngx-translate/core';

@Injectable()
@Pipe({ name: 'translator', pure: false })
export class LocalTranslatorPipe extends TranslatePipe implements PipeTransform {
  constructor(protected localTranslator: LocalTranslatorService, protected cdr: ChangeDetectorRef) {
    super(localTranslator, cdr);
  }
}
