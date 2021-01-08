import { AfterViewInit, ComponentRef, HostBinding, Inject, ViewChild, ViewContainerRef } from '@angular/core';
import { IModuleFlowManager, MODULE_FLOW_MANAGER } from '../../module-flow-manager.interface';

import { FragmentRegistry } from '../fragment-registry';
import { ModuleInstance } from '../shell.models';

export abstract class BaseModuleOutlet implements AfterViewInit {
  @ViewChild('moduleContainer', { read: ViewContainerRef, static: false })
  public viewContainerRef: ViewContainerRef;

  constructor(
    protected moduleInstance: ModuleInstance,
    public fragmentRegistry: FragmentRegistry,
    @Inject(MODULE_FLOW_MANAGER)
    protected moduleFlowManager: IModuleFlowManager
  ) {}

  @HostBinding('class.outlet-container')
  public getOutLetCss(): boolean {
    return true;
  }

  public ngAfterViewInit(): void {
    const componentRef: ComponentRef<unknown> = this.viewContainerRef.createComponent(this.moduleInstance.componentFactory);

    this.moduleInstance.componentRef = componentRef;
    componentRef.location.nativeElement.classList.add('match-parent');

    this.onAfterViewInit();
  }

  protected onAfterViewInit(): void {
    // Virtual method
  }
}
