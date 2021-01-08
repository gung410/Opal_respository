import { BaseModuleOutlet, FragmentRegistry, ModuleFlowManager, ModuleInstance } from '@opal20/infrastructure';

import { Component } from '@angular/core';

@Component({
  selector: 'poc-outlet',
  template: `
    <ng-template [portalOutlet]="fragmentRegistry.get('f-action-bar')"></ng-template>
    <div class="page-content">
      <h3>This is POC Outlet</h3>
      <ng-template [portalOutlet]="fragmentRegistry.get('f-menu')"></ng-template>
      <div class="main-area">
        <ng-template [portalOutlet]="fragmentRegistry.get('f-col1')"></ng-template>
        <ng-container #moduleContainer></ng-container>
        <ng-template [portalOutlet]="fragmentRegistry.get('f-col3')"></ng-template>
      </div>
    </div>
  `
})
export class POCOutletComponent extends BaseModuleOutlet {
  constructor(
    protected moduleInstance: ModuleInstance,
    public fragmentRegistry: FragmentRegistry,
    protected moduleFlowManager: ModuleFlowManager
  ) {
    super(moduleInstance, fragmentRegistry, moduleFlowManager);
  }
}
