import { BaseRoutingModule, ModuleFacadeService } from '@opal20/infrastructure';

import { CommonComponentsModule } from '@opal20/common-components';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './dashboard.component';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DomainComponentsModule } from '@opal20/domain-components';
import { NgModule } from '@angular/core';
import { Router } from '@angular/router';

@NgModule({
  imports: [CommonModule, DashboardRoutingModule, CommonComponentsModule, DomainComponentsModule],
  declarations: [DashboardComponent],
  bootstrap: [DashboardComponent]
})
export class DashboardModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router) {
    super(moduleFacadeService, router);
  }
  protected onInit(): void {
    this.shellManager.registerDefaultFragments();
  }

  protected onDestroy(): void {
    this.shellManager.unregisterDefaultFragments();
  }
}
