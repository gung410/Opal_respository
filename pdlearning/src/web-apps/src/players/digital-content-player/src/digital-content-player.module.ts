import { BaseModuleOutlet, BaseRoutingModule, FunctionModule, ModuleFacadeService } from '@opal20/infrastructure';
import { NgModule, Type } from '@angular/core';

import { CommonComponentsModule } from '@opal20/common-components';
import { DigitalContentPlayerContainerComponent } from './components/digital-content-player-container.component';
import { DigitalContentPlayerOutletComponent } from './digital-content-player-outlet.component';
import { DigitalContentPlayerPageComponent } from './digital-content-player-page.component';
import { DigitalContentPlayerRoutingModule } from './digital-content-player-routing.module';
import { DomainComponentsModule } from '@opal20/domain-components';
import { Router } from '@angular/router';

@NgModule({
  imports: [FunctionModule, CommonComponentsModule, DomainComponentsModule, DigitalContentPlayerRoutingModule],
  declarations: [DigitalContentPlayerOutletComponent, DigitalContentPlayerPageComponent, DigitalContentPlayerContainerComponent],
  providers: [],
  entryComponents: [DigitalContentPlayerOutletComponent],
  bootstrap: [DigitalContentPlayerPageComponent]
})
export class DigitalContentPlayerModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router) {
    super(moduleFacadeService, router);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return DigitalContentPlayerOutletComponent;
  }
}
