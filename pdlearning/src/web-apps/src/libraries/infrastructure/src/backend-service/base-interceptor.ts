import { FW_DISPLAY_SPINNER, FW_INTERCEPTOR_KEYS } from '../constants';
import { HTTP_INTERCEPTORS, HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor, HttpRequest } from '@angular/common/http';

import { Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

export abstract class BaseInterceptor implements HttpInterceptor {
  protected abstract key: string;

  constructor(protected injector: Injector) {}

  /**
   * When a service extends @see BaseBackendService, it can indicates which interceptors will be run or not.
   * This base function gets the FW_INTERCEPTOR_KEYS key in request headers and checkes if this interceptor existed and can run.
   * The FW_INTERCEPTOR_KEYS header is passed from @see BaseBackendService by @see InterceptorRegistry.
   */
  public intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const interceptorsJson: string = req.headers.get(FW_INTERCEPTOR_KEYS);

    if (interceptorsJson) {
      const interceptorKeys: string[] = JSON.parse(interceptorsJson);

      if (interceptorKeys.some(key => key === this.key)) {
        this.processHeaders(req.headers);

        const interceptors: HttpInterceptor[] = this.injector.get(HTTP_INTERCEPTORS, []);

        /**
         * To avoid cross origin headers not eccepted issue in backend side,
         * we must remove the internal headers if the current interceptor is the last one before the request is sent to the server.
         */
        if (this === interceptors[interceptors.length - 1]) {
          const headers: HttpHeaders = req.headers.delete(FW_INTERCEPTOR_KEYS).delete(FW_DISPLAY_SPINNER);

          req = req.clone({ headers: headers });
        }

        return this.handle(req, next).pipe(finalize(() => this.finalize()));
      }
    }

    return next.handle(req).pipe(finalize(() => this.finalize()));
  }

  protected abstract handle(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>>;

  protected processHeaders(headers: HttpHeaders): void {
    // Virtual method
  }

  protected finalize(): void {
    // Virtual method
  }
}
