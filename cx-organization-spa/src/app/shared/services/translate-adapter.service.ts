import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import * as _ from 'lodash';
import { Observable } from 'rxjs';

@Injectable()
export class TranslateAdapterService {
  constructor(private translate: TranslateService) {}
  switchLanguage(language: string, languageName: string): void {
    const existingLanguage = this.translate.store.langs.find(
      (x) => x === language
    );
    if (!existingLanguage) {
      this.mergeLanguage(language);
    } else {
      this.translate.use(language);
      localStorage.setItem('language-code', language);
      localStorage.setItem('language-name', languageName);
    }
  }

  mergeLanguage(language: string): void {
    this.translate
      .getTranslation(language)
      .toPromise()
      .then((originalLanguage) => {
        if (originalLanguage) {
          this.translate
            .getTranslation(`${language}.override`)
            .toPromise()
            .then((overridenLanguage) => {
              if (overridenLanguage) {
                const result = _.merge(originalLanguage, overridenLanguage);
                this.translate.setTranslation(language, result);
                this.translate.use(language);
              }
            })
            .catch((error) => this.translate.use(language));
        }
      });
  }

  setDefaultLanguage(languageCode: string): void {
    this.translate.setDefaultLang(languageCode);
  }

  getCurrentLanguage(): string {
    const languageCode = localStorage.getItem('language-code');
    if (languageCode) {
      return languageCode;
    }

    return 'nb-NO';
  }

  getCurrentLanguageName(): string {
    const languageName = localStorage.getItem('language-name');
    if (languageName) {
      return languageName;
    }

    return 'Norsk';
  }

  getValueBasedOnKey(
    key: string | string[],
    interpolateParams?: any
  ): Observable<any> {
    return this.translate.get(key, interpolateParams);
  }

  getValueImmediately(key: string, interpolateParams?: any): string {
    return this.translate.instant(key, interpolateParams);
  }
}
