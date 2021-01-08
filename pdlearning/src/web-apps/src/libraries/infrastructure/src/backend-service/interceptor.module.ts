import { Injector, NgModule } from '@angular/core';

import { AuthInterceptor } from './interceptors/auth.interceptor';
import { GlobalSpinnerService } from '../spinner/global-spinner.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpResponseInterceptor } from './interceptors/http-response.interceptor';
import { ModalService } from '../services/modal.service';
import { SpinnerInterceptor } from './interceptors/spinner.interceptor';

@NgModule({
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      deps: [Injector],
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpResponseInterceptor,
      deps: [Injector, ModalService],
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: SpinnerInterceptor,
      deps: [Injector, GlobalSpinnerService],
      multi: true
    }
  ]
})
export class InterceptorModule {}
