import { HttpEvent, HttpHandler, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';

import { BaseInterceptor } from '@opal20/infrastructure';
import { Observable } from 'rxjs';

@Injectable()
export class POCExternalUrlInterceptor extends BaseInterceptor {
  protected key: string = 'EXTERNAL_URL_INTERCEPTOR';

  constructor(protected injector: Injector) {
    super(injector);
  }

  public handle(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(req);
  }
}
