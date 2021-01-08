import { BaseModuleOutlet, FragmentRegistry, ModuleFlowManager, ModuleInstance } from '@opal20/infrastructure';

import { Component } from '@angular/core';

@Component({
  selector: 'digital-content-player-page-outlet',
  template: `
    <div class="match-parent row">
      <div class="page-content">
        <div class="flex">
          <ng-container #moduleContainer></ng-container>
        </div>
      </div>
    </div>
  `
})
export class ScormPlayerIntegratorOutletComponent extends BaseModuleOutlet {
  constructor(
    protected moduleInstance: ModuleInstance,
    public fragmentRegistry: FragmentRegistry,
    protected moduleFlowManager: ModuleFlowManager
  ) {
    super(moduleInstance, fragmentRegistry, moduleFlowManager);
  }
}
