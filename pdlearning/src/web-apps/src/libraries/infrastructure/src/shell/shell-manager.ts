import { Injectable, Type } from '@angular/core';
import { ModuleClosingParameters, ModuleFlowParameters } from '../module-flow-parameters';
import { ModuleContext, ModuleInfo, ModuleInstance } from './shell.models';
import { ModuleInfoRegistry, ModuleInstanceRegistry } from './module-registries';

import { AppShell } from './app-shell';
import { BrowserIdleHandler } from '../specials/browser-idle.handler';
import { Fragment } from './fragment';
import { ModuleNotFoundError } from '../errors';

@Injectable()
export class ShellManager {
  public moduleContext: ModuleContext = new ModuleContext();
  public moduleInstanceRegistry: ModuleInstanceRegistry = new ModuleInstanceRegistry();
  private moduleInfoRegistry: ModuleInfoRegistry = new ModuleInfoRegistry();
  private _shell: AppShell;
  private _defaultFragments: { [position: string]: Type<Fragment> } = {};

  public get shell(): AppShell {
    return this._shell;
  }

  constructor(private browserIdleHandler: BrowserIdleHandler) {}

  public init(shell: AppShell): void {
    this._shell = shell;
  }

  public initSpecialHandler(): void {
    if (!AppGlobal.environment.authConfig.ignoredPaths.some(_ => _ === location.pathname)) {
      this.browserIdleHandler.start();
    }
  }

  public registerModule(moduleInfo: ModuleInfo): void {
    if (!this.moduleInfoRegistry.any(moduleInfo.id)) {
      this.moduleInfoRegistry.register(moduleInfo);
    }
  }

  public setDefaultFragments(fragments: { [position: string]: Type<Fragment> }): void {
    this._defaultFragments = fragments;
  }

  public registerDefaultFragments(): void {
    this.registerFragments(this._defaultFragments);
  }

  public registerDefaultFragmentAt(position: string): void {
    const fragment: Type<Fragment> = this._defaultFragments[position];

    if (fragment) {
      this._shell.registerFragment(position, fragment);
    }
  }

  public registerFragment(position: string, type: Type<Fragment>): void {
    this._shell.registerFragment(position, type);
  }

  public registerFragments(fragments: { [position: string]: Type<Fragment> }): void {
    for (const position in fragments) {
      this.registerFragment(position, fragments[position]);
    }
  }

  public unregisterFragment(position: string): void {
    this._shell.unregisterFragment(position);
  }

  public unregisterAllFragments(): void {
    this._shell.unregisterAllFragments();
  }

  public unregisterDefaultFragments(): void {
    this._shell.unregisterAllFragments();
    for (const position in this._defaultFragments) {
      this.unregisterFragment(position);
    }
  }

  public loadModule(parameters: ModuleFlowParameters): Promise<boolean> {
    const moduleClosingParameters: ModuleClosingParameters = new ModuleClosingParameters();

    moduleClosingParameters.ignoreDirtyModuleChecking = parameters.ignoreDirtyModuleChecking;

    if (parameters.forceUnloadExistingModule) {
      return this.checkAndUnloadAllModules(moduleClosingParameters).then(unloadSuccess => {
        if (unloadSuccess) {
          return this.internalLoadModule(parameters).then(() => true);
        }

        return Promise.resolve(false);
      });
    } else {
      return this.internalLoadModule(parameters).then(() => true);
    }
  }

  public internalLoadModule(parameters: ModuleFlowParameters): Promise<boolean> {
    const moduleInfo: ModuleInfo = this.moduleInfoRegistry.get(parameters.moduleId);

    if (!moduleInfo) {
      return Promise.reject(
        new ModuleNotFoundError(
          `Could not find module [${parameters.moduleId}] in registry.
          The reason is maybe you forget to configure in the angular.manifet.json`
        )
      );
    }

    const moduleInstance: ModuleInstance = new ModuleInstance(moduleInfo);

    this.moduleInstanceRegistry.register(moduleInstance);
    parameters.moduleInstance = moduleInstance;

    return this._shell.loadModule(moduleInstance, parameters.outletType, parameters.providers, parameters.injector).then(() => true);
  }

  public checkAndUnloadAllModules(moduleClosingParameters: ModuleClosingParameters = new ModuleClosingParameters()): Promise<boolean> {
    const moduleInstance: ModuleInstance = this.moduleContext.currentModuleInstance;

    return this.checkAndUnloadModule(moduleInstance, moduleClosingParameters).then(unloadSuccess => {
      if (unloadSuccess && this.moduleContext.currentModuleInstance) {
        return this.checkAndUnloadAllModules(moduleClosingParameters);
      }

      return unloadSuccess;
    });
  }

  public checkAndUnloadCurrentModule(): Promise<boolean> {
    const moduleInstance: ModuleInstance = this.moduleContext.currentModuleInstance;

    return this.checkAndUnloadModule(moduleInstance);
  }

  public checkAndUnloadModule(
    moduleInstance: ModuleInstance,
    moduleClosingParameters: ModuleClosingParameters = new ModuleClosingParameters()
  ): Promise<boolean> {
    const canDeactivatePromise: (moduleInstance: ModuleInstance) => Promise<boolean> = moduleClosingParameters.ignoreDirtyModuleChecking
      ? () => Promise.resolve(true)
      : this.canDeactivateModule;

    return canDeactivatePromise(moduleInstance).then(canDeactivate => {
      if (canDeactivate) {
        this.unloadModule(moduleInstance, moduleClosingParameters);
      }

      return Promise.resolve(canDeactivate);
    });
  }

  public unloadModule(
    moduleInstanceOrId: string | ModuleInstance,
    moduleClosingParameters: ModuleClosingParameters = new ModuleClosingParameters()
  ): void {
    let moduleInstance: ModuleInstance | undefined;

    if (moduleInstanceOrId instanceof ModuleInstance) {
      moduleInstance = moduleInstanceOrId;
    } else {
      moduleInstance = this.moduleInstanceRegistry.getModuleByInfoId(moduleInstanceOrId);
    }

    if (!moduleInstance) {
      return;
    }

    this._shell.destroyModule(moduleInstance);
    this.moduleInstanceRegistry.remove(moduleInstance.id);

    const lastModuleInstance: ModuleInstance = this.moduleInstanceRegistry.getLastModuleInstance();

    this.moduleContext.currentModuleInstance = lastModuleInstance;

    if (moduleInstance.onCurrentModuleClosed) {
      moduleInstance.onCurrentModuleClosed(moduleClosingParameters);
    }
  }

  private canDeactivateModule(moduleInstance: ModuleInstance): Promise<boolean> {
    if (moduleInstance && moduleInstance.currentPageComponent && moduleInstance.currentPageComponent.canDeactivate) {
      return moduleInstance.currentPageComponent.canDeactivate().toPromise();
    }

    return Promise.resolve(true);
  }
}
