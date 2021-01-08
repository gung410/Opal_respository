import { BaseModuleOutlet, BaseRoutingModule, FunctionModule, ModuleFacadeService } from '@opal20/infrastructure';
import { Injector, NgModule, Type } from '@angular/core';

import { CommonComponentsModule } from '@opal20/common-components';
import { DomainComponentsModule } from '@opal20/domain-components';
import { FormStandaloneComponent } from './form-standalone.component';
import { FormStandaloneOutletComponent } from './form-standalone-outlet.component';
import { FormStandaloneRoutingModule } from './form-standalone-routing.module';
import { Router } from '@angular/router';

@NgModule({
  imports: [FunctionModule, FormStandaloneRoutingModule, CommonComponentsModule, DomainComponentsModule],
  declarations: [FormStandaloneComponent, FormStandaloneOutletComponent],
  entryComponents: [FormStandaloneOutletComponent],
  bootstrap: [FormStandaloneComponent]
})
export class FormStandaloneModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router, protected injector: Injector) {
    super(moduleFacadeService, router, injector);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return FormStandaloneOutletComponent;
  }

  protected onInit(): void {
    this.shellManager.registerDefaultFragments();
  }

  protected onDestroy(): void {
    this.shellManager.unregisterDefaultFragments();
  }
}
