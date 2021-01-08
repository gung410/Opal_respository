import { ModuleWithProviders, NgModule, Type } from '@angular/core';

import { AngularModule } from './angular.module';
import { ContextDataService } from './services/context-data.service';
import { FormBuilderService } from './form/form-builder.service';
import { GlobalSpinnerService } from './spinner/global-spinner.service';
import { GlobalTranslatorService } from './translation/global-translator.service';
import { InfrastructureModule } from './infrastructure.module';
import { LocalTranslationModule } from './translation/translation.module';
import { ModuleFlowManager } from './module-flow-manager';
import { ModuleInstance } from './shell/shell.models';
import { NavigationService } from './services/navigation.service';
import { PortalModule } from './portal/portal.module';

const FUNCTION_MODULES: Type<unknown>[] = [InfrastructureModule, LocalTranslationModule, AngularModule, PortalModule];

@NgModule({
  imports: [...FUNCTION_MODULES],
  exports: [...FUNCTION_MODULES],
  providers: []
})
export class FunctionModule {
  public static forChild(): ModuleWithProviders {
    return {
      ngModule: FunctionModule,
      providers: [
        { provide: FormBuilderService, deps: [GlobalSpinnerService, GlobalTranslatorService] },
        { provide: ContextDataService, deps: [] },
        { provide: NavigationService, deps: [ModuleInstance, ModuleFlowManager, ContextDataService] }
      ]
    };
  }
}
