import { HttpEvent, HttpHandler, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';

import { AppInfoService } from '../../app-info/app-info.service';
import { BaseInterceptor } from '../base-interceptor';
import { InterceptorType } from '../interceptor-registry';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor extends BaseInterceptor {
  protected key: string = InterceptorType.Authentication;

  constructor(protected injector: Injector) {
    super(injector);
  }

  public handle(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const appInfoService: AppInfoService = this.injector.get(AppInfoService, null);
    const accessToken: string = appInfoService.getAccessToken();

    if (accessToken) {
      const headers = req.headers
        .set('cxToken', '3001:2052')
        .set('Authorization', `Bearer ${accessToken}`)
        .set('Cache-Control', 'no-cache, no-store')
        .set('Pragma', 'no-cache')
        .set('Expires', '-1');

      req = req.clone({ headers });
    }

    return next.handle(req);
  }
}
