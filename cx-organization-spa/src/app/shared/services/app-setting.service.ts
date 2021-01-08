import { Injectable, Injector } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class AppSettingService {
  constructor(private injector: Injector) {}

  getJSON(path: string): Observable<any> {
    return this.injector.get(HttpHelpers).get(path);
  }

  getLanguages(): Observable<any> {
    return this.getJSON('../../assets/appsettings/languages.json');
  }
}
