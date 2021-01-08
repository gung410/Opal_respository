import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { merge } from 'lodash';
import { Observable } from 'rxjs';

@Injectable()
export class TranslateAdapterService {
  constructor(private translate: TranslateService) {}
  public switchLanguage(language: string, languageName: string): void {
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

  public mergeLanguage(language: string): Promise<void> {
    return this.translate
      .getTranslation(language)
      .toPromise()
      .then((originalLanguage) => {
        if (originalLanguage) {
          this.translate
            .getTranslation(`${language}.override`)
            .toPromise()
            .then((overridenLanguage) => {
              if (overridenLanguage) {
                const result = merge(originalLanguage, overridenLanguage);
                this.translate.setTranslation(language, result);
                this.translate.use(language);
              }
            })
            .catch((error) => this.translate.use(language));
        }
      });
  }

  public setDefaultLanguage(languageCode: string): void {
    this.translate.setDefaultLang(languageCode);
  }

  public getCurrentLanguage(): string {
    const languageCode = localStorage.getItem('language-code');
    if (languageCode) {
      return languageCode;
    }

    return 'nb-NO';
  }

  public getCurrentLanguageName(): string {
    const languageName = localStorage.getItem('language-name');
    if (languageName) {
      return languageName;
    }

    return 'Norsk';
  }

  public getValueBasedOnKey(
    key: string | string[],
    interpolateParams?: Object
  ): Observable<any> {
    return this.translate.get(key, interpolateParams);
  }

  public getValueImmediately(key: string, interpolateParams?: Object): string {
    if (!key) {
      return undefined;
    }

    return this.translate.instant(key, interpolateParams);
  }
}
