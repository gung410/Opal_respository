import { BaseModuleOutlet, FragmentRegistry, ModuleFlowManager, ModuleInstance } from '@opal20/infrastructure';

import { Component } from '@angular/core';

@Component({
  selector: 'learner-outlet',
  template: `
    <div class="match-parent column">
      <ng-template [portalOutlet]="fragmentRegistry.get('f-navigation-menu')"></ng-template>
      <div class="page-content">
        <broadcast-message-notification></broadcast-message-notification>
        <div class="d-flex flex-grow-1 flex-shrink-0">
          <ng-container #moduleContainer></ng-container>
        </div>
        <opal-footer></opal-footer>
      </div>
    </div>
  `
})
export class LearnerOutletComponent extends BaseModuleOutlet {
  constructor(
    protected moduleInstance: ModuleInstance,
    public fragmentRegistry: FragmentRegistry,
    protected moduleFlowManager: ModuleFlowManager
  ) {
    super(moduleInstance, fragmentRegistry, moduleFlowManager);
  }
}
