import { BaseModuleOutlet, BaseRoutingModule, FunctionModule, ModuleFacadeService } from '@opal20/infrastructure';
import { NgModule, Type } from '@angular/core';

import { CommonComponentsModule } from '@opal20/common-components';
import { DomainComponentsModule } from '@opal20/domain-components';
import { Router } from '@angular/router';
import { ScormPlayerContainerComponent } from './components/scorm-player-container.component';
import { ScormPlayerIntegratorComponent } from './scorm-player-integrator.component';
import { ScormPlayerIntegratorOutletComponent } from './scorm-player-integrator-outlet.component';
import { ScormPlayerIntegratorRoutingModule } from './scorm-player-integrator-routing.module';

@NgModule({
  imports: [FunctionModule, CommonComponentsModule, DomainComponentsModule, ScormPlayerIntegratorRoutingModule],
  declarations: [ScormPlayerIntegratorOutletComponent, ScormPlayerIntegratorComponent, ScormPlayerContainerComponent],
  providers: [],
  entryComponents: [ScormPlayerIntegratorOutletComponent],
  bootstrap: [ScormPlayerIntegratorComponent]
})
export class ScormPlayerIntegratorModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router) {
    super(moduleFacadeService, router);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return ScormPlayerIntegratorOutletComponent;
  }
}
