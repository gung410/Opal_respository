import { AuthService, UrlHelperService } from '@opal20/authentication';

import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CalendarIntergrationService {
  public isInternalIntergration: BehaviorSubject<boolean> = new BehaviorSubject(false);
  public currentUser: BehaviorSubject<object> = new BehaviorSubject(AppGlobal.user);

  protected urlParameters: { [key: string]: string | number | unknown };

  constructor(protected urlHelper: UrlHelperService, private authService: AuthService) {}

  public verifyIntergrationUrl(): void {
    if (this.isInternalIntergration.getValue()) {
      return;
    }

    this.urlParameters = this.GetParams();

    if (!this.urlParameters.accessToken) {
      console.warn('The access token was missed');
      return;
    }

    this.authService.setAccessToken(this.urlParameters.accessToken as string);
  }

  public GetParams(): { [key: string]: string | number | unknown } {
    let urlParamString = location.search;
    if (urlParamString && urlParamString.startsWith('?')) {
      urlParamString = urlParamString.substr(1, urlParamString.length - 1);
    }
    return this.urlHelper.parseQueryString(urlParamString) as {
      [key: string]: string | number | unknown;
    };
  }

  public setInternalMode(isInternalIntergration: true): void {
    this.isInternalIntergration.next(isInternalIntergration);
  }
}
