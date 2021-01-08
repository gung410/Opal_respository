import { ModuleWithProviders, NgModule } from '@angular/core';

import { GlobalSpinnerService } from './global-spinner.service';
import { SpinnerDirective } from './spinner.directive';
import { SpinnerService } from './spinner.service';

@NgModule({
  declarations: [SpinnerDirective],
  exports: [SpinnerDirective]
})
export class SpinnerModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: SpinnerModule,
      providers: [GlobalSpinnerService]
    };
  }

  public static forChild(): ModuleWithProviders {
    return {
      ngModule: SpinnerModule,
      providers: [SpinnerService]
    };
  }
}
