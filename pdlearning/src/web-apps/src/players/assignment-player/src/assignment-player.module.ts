import { BaseModuleOutlet, BaseRoutingModule, FunctionModule, ModuleFacadeService } from '@opal20/infrastructure';
import { NgModule, Type } from '@angular/core';

import { AssignmentPlayerComponent } from './assignment-player.component';
import { AssignmentPlayerOutletComponent } from './assignment-player-outlet.component';
import { AssignmentPlayerRoutingModule } from './assignment-player-routing.module';
import { CommonComponentsModule } from '@opal20/common-components';
import { DomainComponentsModule } from '@opal20/domain-components';
import { Router } from '@angular/router';

@NgModule({
  imports: [FunctionModule, AssignmentPlayerRoutingModule, CommonComponentsModule, DomainComponentsModule],
  declarations: [AssignmentPlayerComponent, AssignmentPlayerOutletComponent],
  providers: [],
  entryComponents: [AssignmentPlayerOutletComponent],
  bootstrap: [AssignmentPlayerComponent]
})
export class AssignmentPlayerModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router) {
    super(moduleFacadeService, router);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return AssignmentPlayerOutletComponent;
  }
}
