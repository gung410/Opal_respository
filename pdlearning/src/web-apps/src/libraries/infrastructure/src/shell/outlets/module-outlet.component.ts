import { Component, Inject } from '@angular/core';
import { IModuleFlowManager, MODULE_FLOW_MANAGER } from '../../module-flow-manager.interface';

import { BaseModuleOutlet } from './base-module-outlet';
import { FragmentRegistry } from '../fragment-registry';
import { ModuleInstance } from '../shell.models';

@Component({
  selector: 'module-outlet',
  template: `
    <div class="f-row2">
      <div class="f-main">
        <ng-container #moduleContainer></ng-container>
      </div>
    </div>
  `
})
export class ModuleOutletComponent extends BaseModuleOutlet {
  constructor(
    protected moduleInstance: ModuleInstance,
    public fragmentRegistry: FragmentRegistry,
    @Inject(MODULE_FLOW_MANAGER)
    protected moduleFlowManager: IModuleFlowManager
  ) {
    super(moduleInstance, fragmentRegistry, moduleFlowManager);
  }
}
