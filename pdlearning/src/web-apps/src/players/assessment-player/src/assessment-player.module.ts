import { BaseModuleOutlet, BaseRoutingModule, FunctionModule, ModuleFacadeService } from '@opal20/infrastructure';
import { NgModule, Type } from '@angular/core';

import { AssessmentPlayerComponent } from './assessment-player.component';
import { AssessmentPlayerOutletComponent } from './assessment-player-outlet.component';
import { AssessmentPlayerRoutingModule } from './assessment-player-routing.module';
import { CommonComponentsModule } from '@opal20/common-components';
import { DomainComponentsModule } from '@opal20/domain-components';
import { Router } from '@angular/router';

@NgModule({
  imports: [FunctionModule, AssessmentPlayerRoutingModule, CommonComponentsModule, DomainComponentsModule],
  declarations: [AssessmentPlayerComponent, AssessmentPlayerOutletComponent],
  providers: [],
  entryComponents: [AssessmentPlayerOutletComponent],
  bootstrap: [AssessmentPlayerComponent]
})
export class AssessmentPlayerModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router) {
    super(moduleFacadeService, router);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return AssessmentPlayerOutletComponent;
  }
}
