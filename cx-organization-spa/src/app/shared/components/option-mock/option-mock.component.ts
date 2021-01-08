import {
  MissingTranslationHandler,
  MissingTranslationHandlerParams
} from '@ngx-translate/core';

export class TranslateHandlerCustom implements MissingTranslationHandler {
  private response: string;

  handle(params: MissingTranslationHandlerParams): string {
    return 'some translated text'; // here u can return translation
  }
}
