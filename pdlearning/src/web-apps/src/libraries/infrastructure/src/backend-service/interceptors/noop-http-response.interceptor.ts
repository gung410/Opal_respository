import { HttpEvent, HttpHandler, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';

import { BaseInterceptor } from '../base-interceptor';
import { Observable } from 'rxjs';

@Injectable()
export class NoopHttpResponseInterceptor extends BaseInterceptor {
  protected key: string = 'NOOP_HTTP_RESPONSE_INTERCEPTOR';

  constructor(protected injector: Injector) {
    super(injector);
  }

  public handle(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(req);
  }
}
