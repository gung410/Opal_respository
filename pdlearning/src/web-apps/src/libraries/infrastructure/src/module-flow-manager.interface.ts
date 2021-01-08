import { IModuleParameters, ModuleFlowParameters } from './module-flow-parameters';

import { InjectionToken } from '@angular/core';
import { ModuleFlowManager } from './module-flow-manager';

export const MODULE_FLOW_MANAGER: InjectionToken<ModuleFlowManager> = new InjectionToken('fw.module-flow-manager');

export interface IModuleFlowManager {
  bootstrapMainApp(): Promise<void>;
  openModule(moduleId: string, moduleParameters?: IModuleParameters): Promise<void>;
  internalOpenModule(moduleId: string, parameters?: ModuleFlowParameters): Promise<void>;
  checkAndUnloadCurrentModule(): Promise<boolean>;
}
