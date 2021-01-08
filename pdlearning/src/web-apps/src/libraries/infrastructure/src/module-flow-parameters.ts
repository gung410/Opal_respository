import { Injector, StaticProvider, Type } from '@angular/core';

import { BaseModuleOutlet } from './shell/outlets/base-module-outlet';
import { MODULE_INPUT_DATA } from './constants';
import { ModuleDataService } from './services/context-data.service';
import { ModuleInstance } from './shell/shell.models';

export interface IModuleParameters {
  outletType?: Type<BaseModuleOutlet>;
  data?: unknown;
}

export class ModuleFlowParameters {
  public loadSucceded: boolean;

  public outletType: Type<BaseModuleOutlet>;

  public moduleInstance: ModuleInstance;

  public providers: StaticProvider[] = [];

  public injector?: Injector;

  public ignoreDirtyModuleChecking: boolean;

  public forceUnloadExistingModule: boolean;

  private _moduleId: string;

  public get moduleId(): string {
    return (this._moduleId || 'dashboard').toLowerCase();
  }

  public set moduleId(value: string) {
    this._moduleId = value;
  }

  public from(moduleParameters?: IModuleParameters): void {
    if (!moduleParameters) {
      moduleParameters = {
        data: null
      };
    }

    this.outletType = moduleParameters.outletType;

    const moduleDataService: ModuleDataService = new ModuleDataService();

    moduleDataService.setData(MODULE_INPUT_DATA, moduleParameters.data);
    this.providers.push({
      provide: ModuleDataService,
      useValue: moduleDataService
    });
  }
}

export class ModuleClosingParameters {
  public ignoreDirtyModuleChecking: boolean = false;
}
