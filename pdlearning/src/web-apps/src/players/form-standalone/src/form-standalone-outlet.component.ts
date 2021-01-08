import { BaseModuleOutlet, FragmentRegistry, ModuleFlowManager, ModuleInstance } from '@opal20/infrastructure';

import { Component } from '@angular/core';

@Component({
  selector: 'opal-outlet',
  template: `
    <div class="match-parent column">
      <div class="page-content">
        <div class="d-flex flex-grow-1 flex-shrink-0">
          <ng-container #moduleContainer></ng-container>
        </div>
        <opal-footer></opal-footer>
      </div>
    </div>
  `
})
export class FormStandaloneOutletComponent extends BaseModuleOutlet {
  constructor(
    protected moduleInstance: ModuleInstance,
    public fragmentRegistry: FragmentRegistry,
    protected moduleFlowManager: ModuleFlowManager
  ) {
    super(moduleInstance, fragmentRegistry, moduleFlowManager);
  }
}
