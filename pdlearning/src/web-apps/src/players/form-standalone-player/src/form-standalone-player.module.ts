import { BaseModuleOutlet, BaseRoutingModule, FunctionModule, ModuleFacadeService } from '@opal20/infrastructure';
import { Injector, NgModule, Type } from '@angular/core';

import { CommonComponentsModule } from '@opal20/common-components';
import { DomainComponentsModule } from '@opal20/domain-components';
import { FormStandalonePlayerComponent } from './form-standalone-player.component';
import { FormStandalonePlayerOutletComponent } from './form-standalone-player-outlet.component';
import { FormStandalonePlayerRoutingModule } from './form-standalone-player-routing.module';
import { Router } from '@angular/router';

@NgModule({
  imports: [FunctionModule, FormStandalonePlayerRoutingModule, CommonComponentsModule, DomainComponentsModule],
  declarations: [FormStandalonePlayerComponent, FormStandalonePlayerOutletComponent],
  entryComponents: [FormStandalonePlayerOutletComponent],
  bootstrap: [FormStandalonePlayerComponent]
})
export class FormStandalonePlayerModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router, protected injector: Injector) {
    super(moduleFacadeService, router, injector);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return FormStandalonePlayerOutletComponent;
  }
}
