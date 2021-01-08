import { IModuleParameters, ModuleFlowParameters } from './module-flow-parameters';

import { GlobalTranslatorService } from './translation/global-translator.service';
import { IModuleFlowManager } from './module-flow-manager.interface';
import { Injectable } from '@angular/core';
import { ShellManager } from './shell/shell-manager';

@Injectable()
export class ModuleFlowManager implements IModuleFlowManager {
  constructor(private shellManager: ShellManager, private globalTranslator: GlobalTranslatorService) {}

  public bootstrapMainApp(): Promise<void> {
    const parameters: ModuleFlowParameters = new ModuleFlowParameters();

    return Promise.resolve()
      .then(() => this.initTranslation())
      .then(() => this.doBootstrap(parameters))
      .catch((error: Error) => this.handleError(error, parameters));
  }

  public openModule(moduleId: string, moduleParameters?: IModuleParameters): Promise<void> {
    const parameters: ModuleFlowParameters = new ModuleFlowParameters();

    parameters.moduleId = moduleId;
    parameters.forceUnloadExistingModule = true;
    parameters.from(moduleParameters);

    return this.loadModule(parameters);
  }

  /**
   * This function is for internal use only, please don't call it.
   * @access Internal FW
   * @param moduleId The module id.
   * @param parameters The module flow paramters.
   */
  public internalOpenModule(moduleId: string, parameters?: ModuleFlowParameters): Promise<void> {
    parameters.moduleId = moduleId;

    return this.loadModule(parameters);
  }

  public checkAndUnloadCurrentModule(): Promise<boolean> {
    return this.shellManager.checkAndUnloadCurrentModule();
  }

  private loadModule(parameters: ModuleFlowParameters): Promise<void> {
    return this.shellManager.loadModule(parameters).then(loadSucceded => {
      parameters.loadSucceded = loadSucceded;
    });
  }

  private initTranslation(): Promise<void> {
    this.globalTranslator.addLangs(['en', 'vi']);

    return this.globalTranslator.use('en').toPromise();
  }

  private doBootstrap(parameters: ModuleFlowParameters): Promise<void> {
    this.shellManager.initSpecialHandler();

    return Promise.resolve();
  }

  private handleError(error: Error, parameters: ModuleFlowParameters): void {
    // TODO
  }
}
