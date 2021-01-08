import { BaseModuleOutlet, BaseRoutingModule, FunctionModule, ModuleFacadeService, TranslationModule } from '@opal20/infrastructure';
import { Injector, NgModule, Type } from '@angular/core';

import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NumericTextBoxModule } from '@progress/kendo-angular-inputs';
import { POCAppInfoComponent } from './features/app-info/poc-app-info.component';
import { POCBackendComponent } from './features/backend-service/poc-backend.component';
import { POCBackendService } from './features/backend-service/poc-backend.service';
import { POCComponent } from './poc.component';
import { POCDialogComponent } from './features/dialog/poc-dialog.component';
import { POCDialogPageComponent } from './features/dialog/poc-dialog-page.component';
import { POCExternalUrlInterceptor } from './features/backend-service/poc-external-url.interceptor';
import { POCModalComponent } from './features/modal/poc-modal.component';
import { POCMultiFormComponent } from './features/multi-form/poc-multi-form.component';
import { POCNavigationComponent } from './features/navigation/poc-navigation.component';
import { POCNavigationDataComponent } from './features/navigation/poc-navigation-data.component';
import { POCOutletComponent } from './poc-outlet.component';
import { POCRoutingModule } from './poc-routing.module';
import { POCSingleFormComponent } from './features/single-form/poc-single-form.component';
import { POCSingleFormService } from './services/poc-single-form.service';
import { POCSpinnerComponent } from './features/spinner/poc-spinner.component';
import { POCTranslationComponent } from './features/translation/poc-translation.component';
import { Router } from '@angular/router';
import { TooltipModule } from '@progress/kendo-angular-tooltip';

@NgModule({
  imports: [
    FunctionModule,
    POCRoutingModule,
    TranslationModule.registerModules([{ moduleId: 'poc' }]),
    NumericTextBoxModule,
    TooltipModule
  ],
  declarations: [
    POCComponent,
    POCOutletComponent,
    POCSpinnerComponent,
    POCNavigationComponent,
    POCNavigationDataComponent,
    POCTranslationComponent,
    POCAppInfoComponent,
    POCBackendComponent,
    POCModalComponent,
    POCSingleFormComponent,
    POCMultiFormComponent,
    POCDialogPageComponent,
    POCDialogComponent
  ],
  entryComponents: [POCOutletComponent, POCDialogComponent],
  exports: [POCOutletComponent, POCDialogComponent],
  bootstrap: [POCComponent],
  providers: [
    POCBackendService,
    POCSingleFormService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: POCExternalUrlInterceptor,
      deps: [Injector],
      multi: true
    }
  ]
})
export class POCModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router) {
    super(moduleFacadeService, router);
  }

  protected onInit(): void {
    this.shellManager.registerDefaultFragments();
  }

  protected onDestroy(): void {
    this.shellManager.unregisterDefaultFragments();
  }

  get outletType(): Type<BaseModuleOutlet> {
    return POCOutletComponent;
  }
}
