import { HttpEvent, HttpHandler, HttpHeaders, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';

import { BaseInterceptor } from '../base-interceptor';
import { FW_DISPLAY_SPINNER } from '../../constants';
import { GlobalSpinnerService } from '../../spinner/global-spinner.service';
import { InterceptorType } from '../interceptor-registry';
import { Observable } from 'rxjs';

@Injectable()
export class SpinnerInterceptor extends BaseInterceptor {
  protected key: string = InterceptorType.Spinner;
  private displaySpinner: boolean = false;

  constructor(protected injector: Injector, private globalSpinnerService: GlobalSpinnerService) {
    super(injector);
  }

  public handle(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (this.displaySpinner) {
      this.globalSpinnerService.show();

      return next.handle(req);
    }

    return next.handle(req);
  }

  protected processHeaders(headers: HttpHeaders): void {
    this.displaySpinner = headers.get(FW_DISPLAY_SPINNER) === 'true';
  }

  protected finalize(): void {
    this.displaySpinner = false;
    this.globalSpinnerService.hide();
  }
}
