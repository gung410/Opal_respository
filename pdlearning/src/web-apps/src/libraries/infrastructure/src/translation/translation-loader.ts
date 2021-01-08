import { IModuleInfo, IResource } from './translation.models';
import { Observable, forkJoin, of } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TranslateLoader } from '@ngx-translate/core';
import { map } from 'rxjs/operators';

@Injectable()
export class LocalizationService {
  private localizationUrl: string = './assets/localization';

  constructor(private http: HttpClient) {}

  public getResourceCollection(moduleInfoCollections: IModuleInfo[][], lang: string): Observable<IResource | null> {
    const moduleInfoCollection: IModuleInfo[] = [];

    for (const collection of moduleInfoCollections) {
      for (const moduleInfo of collection) {
        moduleInfoCollection.push(moduleInfo);
      }
    }

    if (moduleInfoCollection.length === 0) {
      return of(null);
    }

    return forkJoin(moduleInfoCollection.map(moduleInfo => this.getResource(moduleInfo, lang))).pipe(
      map(resources => {
        let returnResource: IResource = {};

        resources.forEach(resource => {
          returnResource = Object.assign(returnResource, resource);
        });

        return returnResource;
      })
    );
  }

  private getResource(moduleInfo: IModuleInfo, lang: string): Observable<IResource> {
    return this.http.get<IResource>(`${this.localizationUrl}/${moduleInfo.moduleId}.${lang}.json`);
  }
}

export class TranslationLoader extends TranslateLoader {
  constructor(public localization: LocalizationService, public moduleInfoCollections: IModuleInfo[][]) {
    super();
  }

  public getTranslation(lang: string): Observable<IResource | null> {
    return this.localization.getResourceCollection(this.moduleInfoCollections, lang);
  }
}

export function createTranslationLoader(localization: LocalizationService, moduleInfoCollections: IModuleInfo[][]): TranslationLoader {
  return new TranslationLoader(localization, moduleInfoCollections);
}
