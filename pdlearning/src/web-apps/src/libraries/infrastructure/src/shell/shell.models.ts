import { ComponentFactory, ComponentRef, Injectable, NgModuleRef } from '@angular/core';

import { ModuleClosingParameters } from '../module-flow-parameters';
import { ModuleFacadeService } from '../services/module-facade.service';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

export type ModuleState = 'initialized' | 'loading' | 'loaded' | 'error';

export interface ICanDeactivateComponent {
  canDeactivate: () => Observable<boolean>;
}

export class ModuleContext {
  public currentModuleInstance: ModuleInstance;

  public clear(): void {
    delete this.currentModuleInstance;
  }
}

export class ModuleInfo {
  /**
   * A class to represent an module.
   * @param loadNgModule Delegate method of loading ng module
   * @param id The module id. E.g.: AbcModule
   */
  constructor(public id: string, public loadNgModule: () => Promise<System.Module>) {}
}

/**
 * Contains all information about the module loaded by the dynamic bootstrapping process at @see AppShell
 * Basically, one module will be composed by:
 * 1. Module Info
 * 2. Module Ref: the instance of the module created by the dyanmic bootstrapping process
 * 3. An outlet container which contains outlet fragments.
 * 4. Component Factory to resolve and create Component Ref (also known as Screen Component)
 * 5. Component Ref (aka screen component)
 */
@Injectable()
export class ModuleInstance {
  /**
   * Auto generated id
   */
  public id: string;

  public moduleInfo: ModuleInfo;

  /**
   * Hodling the instance of Module after dynamic bootstrapping module process
   */
  public moduleRef: NgModuleRef<unknown>;

  /**
   * Holding the instance of Outlet component
   */
  public outletComponentRef: ComponentRef<unknown>;

  /**
   * ComponentFactory is used to create componentRef
   */
  public componentFactory: ComponentFactory<unknown>;

  /**
   * After the outlet was created, it will create the Module Component and assign that instance to this variable.
   * Refer this process at @see BaseModuleOutlet
   */
  public componentRef: ComponentRef<unknown>;

  /**
   * This instance was assigned after dynamic bootstrapping module process at @see AppShell
   */
  public router: Router;

  /**
   * All common services will be provided through @see ModuleFacadeService
   * This instance was assigned after dynamic bootstrapping module process at @see AppShell
   */
  public facadeService: ModuleFacadeService;

  /**
   * When a component was bootstraped, the system assigns the instance to this variable.
   * This job will be done in @see BaseScreenComponent
   */
  public currentPageComponent: ICanDeactivateComponent;

  /**
   * Describe the default routing path of a module.
   * E.g.,: s01
   */
  public defaultPath: string;
  public contextDataKey: string;

  public onCurrentModuleClosed: (moduleClosingParameters: ModuleClosingParameters) => void;

  private _state: ModuleState = 'initialized';

  constructor(moduleInfo: ModuleInfo) {
    this.moduleInfo = moduleInfo;
    this._state = 'loading';
  }

  get state(): ModuleState {
    return this._state;
  }

  public isLoading(): boolean {
    return this._state === 'loading';
  }

  public markAsLoading(): void {
    this._state = 'loading';
  }

  public markAsLoaded(): void {
    this._state = 'loaded';
  }

  public stopChangeDetection(): void {
    if (this.outletComponentRef && this.outletComponentRef.changeDetectorRef) {
      this.outletComponentRef.changeDetectorRef.detach();
    }
  }

  public resumeChangeDetection(): void {
    if (this.outletComponentRef && this.outletComponentRef.changeDetectorRef) {
      this.outletComponentRef.changeDetectorRef.reattach();
    }
  }

  public hide(): void {
    if (this.outletComponentRef && this.outletComponentRef.location) {
      (this.outletComponentRef.location.nativeElement as HTMLElement).style.display = 'none';
    }
  }

  public show(): void {
    if (this.outletComponentRef && this.outletComponentRef.location) {
      (this.outletComponentRef.location.nativeElement as HTMLElement).style.display = '';
    }
  }
}
