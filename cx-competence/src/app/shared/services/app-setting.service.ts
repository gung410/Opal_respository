import { Observable } from 'rxjs/Observable';
import { Injectable, Injector } from '@angular/core';
import { environment } from 'app-environments/environment';
import { HttpHelpers } from 'app-utilities/httpHelpers';
declare var CONFIG: any;
@Injectable()
export class AppSettingService {
  constructor(private injector: Injector) {}

  public getJSON(path: string): Observable<any> {
    return this.injector.get(HttpHelpers).get(path);
  }

  public getLanguages() {
    let languageFileUri = 'assets/appsettings/languages.json';
    languageFileUri = environment.VirtualPath
      ? `/${languageFileUri}`
      : `../../${languageFileUri}`;
    return this.getJSON(languageFileUri);
  }
}
