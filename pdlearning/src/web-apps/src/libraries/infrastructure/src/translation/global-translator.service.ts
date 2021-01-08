import {
  MissingTranslationHandler,
  TranslateCompiler,
  TranslateLoader,
  TranslateParser,
  TranslateService,
  TranslateStore
} from '@ngx-translate/core';

import { ITranslationParams } from './translation.models';
import { Injectable } from '@angular/core';

@Injectable()
export class GlobalTranslatorService extends TranslateService {
  constructor(
    public store: TranslateStore,
    public currentLoader: TranslateLoader,
    public compiler: TranslateCompiler,
    public parser: TranslateParser,
    public missingTranslationHandler: MissingTranslationHandler
  ) {
    super(store, currentLoader, compiler, parser, missingTranslationHandler);
  }

  public translate(key: string | string[], params?: ITranslationParams): string {
    return this.instant(key, params);
  }
}
