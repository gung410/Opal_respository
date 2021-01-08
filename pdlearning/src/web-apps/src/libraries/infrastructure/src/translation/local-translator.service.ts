import { Injectable, OnDestroy } from '@angular/core';
import {
  LangChangeEvent,
  MissingTranslationHandler,
  TranslateCompiler,
  TranslateLoader,
  TranslateParser,
  TranslateService,
  TranslateStore
} from '@ngx-translate/core';

import { GlobalTranslatorService } from './global-translator.service';
import { ITranslationParams } from './translation.models';
import { SubscriptionCollection } from '../subscribable';

@Injectable()
export class LocalTranslatorService extends TranslateService implements OnDestroy {
  private _subscriptionCollection: SubscriptionCollection = new SubscriptionCollection();

  constructor(
    public globalTranslator: GlobalTranslatorService,
    public store: TranslateStore,
    public currentLoader: TranslateLoader,
    public compiler: TranslateCompiler,
    public parser: TranslateParser,
    public missingTranslationHandler: MissingTranslationHandler
  ) {
    super(store, currentLoader, compiler, parser, missingTranslationHandler, true, true);
  }

  public ngOnDestroy(): void {
    this._subscriptionCollection.clear();
  }

  public init(): Promise<void> {
    this.defaultLang = this.globalTranslator.defaultLang;
    this.langs = this.globalTranslator.langs;

    this._subscriptionCollection.add(this.globalTranslator.onLangChange, (params: LangChangeEvent) => {
      this.use(params.lang);
    });

    return this.use(this.globalTranslator.currentLang).toPromise();
  }

  public translate(key: string | string[], params?: ITranslationParams): string {
    return this.instant(key, params);
  }

  public translateCommon(key: string | string[], params?: ITranslationParams): string {
    return this.globalTranslator.translate(key, params);
  }
}
