import { NgModule, Type } from '@angular/core';

import { BrowserIdleHandler } from './specials/browser-idle.handler';
import { CommonModule } from '@angular/common';
import { MODULE_FLOW_MANAGER } from './module-flow-manager.interface';
import { ModuleCompiler } from './shell/module-compiler';
import { ModuleFlowManager } from './module-flow-manager';
import { ModuleOutletComponent } from './shell/outlets/module-outlet.component';
import { PortalModule } from './portal/portal.module';
import { ShellManager } from './shell/shell-manager';

const SHELL_COMPONENTS: Type<unknown>[] = [ModuleOutletComponent];

@NgModule({
  imports: [CommonModule, PortalModule],
  declarations: [...SHELL_COMPONENTS],
  entryComponents: [...SHELL_COMPONENTS],
  exports: [...SHELL_COMPONENTS, PortalModule],
  providers: [
    ShellManager,
    ModuleCompiler,
    ModuleFlowManager,
    {
      provide: MODULE_FLOW_MANAGER,
      useExisting: ModuleFlowManager
    },
    BrowserIdleHandler
  ]
})
export class ShellModule {}
