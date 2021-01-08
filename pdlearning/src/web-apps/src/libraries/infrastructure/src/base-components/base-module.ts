import { BaseModuleOutlet } from '..';
import { Fragment } from '../shell/fragment';
import { ModuleOutletComponent } from '../shell/outlets/module-outlet.component';
import { ShellManager } from '../shell/shell-manager';
import { Subscribable } from '../subscribable';
import { Type } from '@angular/core';

export abstract class BaseModule extends Subscribable {
  constructor(protected shellManager: ShellManager) {
    super();

    this.shellManager.registerFragments(this.fragments);

    this.onInit();
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return ModuleOutletComponent;
  }

  protected get fragments(): { [position: string]: Type<Fragment> } {
    return {};
  }

  protected onInit(): void {
    // Virtual
  }

  protected onDestroy(): void {
    // Virtual
  }
}
