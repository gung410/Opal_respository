import { BaseModuleOutlet, FragmentRegistry, ModuleFlowManager, ModuleInstance } from '@opal20/infrastructure';

import { Component } from '@angular/core';

@Component({
  selector: 'opal-outlet',
  template: `
    <div class="match-parent column">
      <ng-template [portalOutlet]="fragmentRegistry.get('f-navigation-menu')"></ng-template>
      <div class="page-content">
        <broadcast-message-notification></broadcast-message-notification>
        <ng-template [portalOutlet]="fragmentRegistry.get('f-app-toolbar')"></ng-template>
        <div class="d-flex flex-grow-1 flex-shrink-0">
          <ng-container #moduleContainer></ng-container>
        </div>
        <opal-footer></opal-footer>
      </div>
    </div>
  `
})
export class OpalOutletComponent extends BaseModuleOutlet {
  constructor(
    protected moduleInstance: ModuleInstance,
    public fragmentRegistry: FragmentRegistry,
    protected moduleFlowManager: ModuleFlowManager
  ) {
    super(moduleInstance, fragmentRegistry, moduleFlowManager);
  }
}
